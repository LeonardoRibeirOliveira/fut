package fhirut.validator;

import fhirut.exceptions.FhirValidationException;
import fhirut.model.*;
import org.hl7.fhir.r4.model.*;
import org.hl7.fhir.r4.formats.JsonParser;

import java.io.*;
import java.nio.charset.StandardCharsets;
import java.util.*;
import java.util.concurrent.TimeUnit;

public class FhirValidator {
    private static final String VALIDATOR_JAR = "prototipo_em_java/lib/validator_cli.jar";
    private static final String FHIR_VERSION = "4.0.1";
    private static final int TIMEOUT_SECONDS = 30;

    public TestResult validate(TestDefinition testDefinition) throws FhirValidationException {
        Objects.requireNonNull(testDefinition, "TestDefinition não pode ser nulo");

        // Verifica se o validador existe
        File validatorFile = new File(VALIDATOR_JAR);
        if (!validatorFile.exists()) {
            throw new FhirValidationException(
                    "Validator_cli.jar não encontrado em: " + validatorFile.getAbsolutePath(),
                    "Por favor, baixe o validator_cli.jar da página oficial da HL7 e coloque na pasta lib/",
                    testDefinition.getTestId()
            );
        }

        try {
            // Configura o ambiente
            Map<String, String> env = new HashMap<>(System.getenv());
            env.put("JAVA_TOOL_OPTIONS", "-Dfile.encoding=UTF-8");

            // Prepara o comando
            List<String> command = buildCommand(testDefinition);

            // Executa o validador com logging detalhado
            System.out.println("Executando comando: " + String.join(" ", command));
            ProcessResult result = executeCommand(command, env);
            System.out.println("Saída do validador:\n" + result.getOutput());

            // Processa o resultado
            return processResult(testDefinition, result);

        } catch (IOException | InterruptedException e) {
            throw new FhirValidationException(
                    "Erro ao executar validador: " + e.getMessage(),
                    "Verifique se o Java está instalado e acessível no PATH",
                    testDefinition.getTestId()
            );
        }
    }

    private List<String> buildCommand(TestDefinition definition) throws FhirValidationException {
        List<String> command = new ArrayList<>();
        command.add("java");
        command.add("-Dfile.encoding=UTF-8");
        command.add("-jar");
        command.add(VALIDATOR_JAR);
        command.add("-version");
        command.add(FHIR_VERSION);

        // Adiciona parâmetros de contexto
        if (definition.getContext() != null) {
            TestContext context = definition.getContext();

            if (context.getIgs() != null) {
                for (String ig : context.getIgs()) {
                    command.add("-ig");
                    command.add(ig);
                }
            }

            if (context.getProfiles() != null) {
                for (String profile : context.getProfiles()) {
                    command.add("-profile");
                    command.add(profile);
                }
            }
        }

        // Adiciona o arquivo de instância
        File instanceFile = definition.getInstancePath();
        if (instanceFile == null || !instanceFile.exists()) {
            throw new FhirValidationException(
                    "Arquivo de instância não encontrado: " +
                            (instanceFile != null ? instanceFile.getPath() : "null"),
                    "Verifique o caminho do arquivo no teste YAML",
                    definition.getTestId()
            );
        }
        command.add(instanceFile.getAbsolutePath());

        return command;
    }

    private ProcessResult executeCommand(List<String> command, Map<String, String> env)
            throws IOException, InterruptedException {

        ProcessBuilder builder = new ProcessBuilder(command);
        builder.environment().putAll(env);
        builder.redirectErrorStream(true); // Redireciona stderr para stdout

        Process process = builder.start();

        // Lê a saída enquanto o processo está executando
        String output = readOutput(process.getInputStream());

        // Espera pelo término do processo
        boolean finished = process.waitFor(TIMEOUT_SECONDS, TimeUnit.SECONDS);
        if (!finished) {
            process.destroyForcibly();
            throw new IOException("Processo excedeu o tempo limite de " + TIMEOUT_SECONDS + " segundos");
        }

        return new ProcessResult(output, process.exitValue());
    }

    private String readOutput(InputStream inputStream) throws IOException {
        try (BufferedReader reader = new BufferedReader(
                new InputStreamReader(inputStream, StandardCharsets.UTF_8))) {

            StringBuilder output = new StringBuilder();
            String line;
            while ((line = reader.readLine()) != null) {
                output.append(line).append("\n");
            }
            return output.toString();
        }
    }

    private TestResult processResult(TestDefinition definition, ProcessResult result)
            throws FhirValidationException {

        if (result.getExitCode() != 0) {
            throw new FhirValidationException(
                    "Validador retornou código de erro: " + result.getExitCode(),
                    result.getOutput(),
                    definition.getTestId()
            );
        }

        try {
            OperationOutcome outcome = (OperationOutcome) new JsonParser().parse(result.getOutput());
            TestResult testResult = new TestResult();
            testResult.setTestId(definition.getTestId());
            testResult.setOutcome(outcome);

            if (definition.getExpectedResults() != null) {
                boolean passed = compareResults(outcome, definition.getExpectedResults(), testResult);
                testResult.setPassed(passed);
            }

            return testResult;

        } catch (Exception e) {
            throw new FhirValidationException(
                    "Erro ao processar saída do validador: " + e.getMessage(),
                    result.getOutput(),
                    definition.getTestId()
            );
        }
    }

    private boolean compareResults(OperationOutcome actual, ExpectedResults expected, TestResult result) {
        List<ValidationDiff> diffs = new ArrayList<>();
        boolean hasErrors = actual.getIssue().stream()
                .anyMatch(issue -> issue.getSeverity() == OperationOutcome.IssueSeverity.ERROR);

        // Verifica status geral
        boolean statusMatches = ("error".equalsIgnoreCase(expected.getStatus()) && hasErrors) ||
                ("success".equalsIgnoreCase(expected.getStatus()) && !hasErrors);

        if (!statusMatches) {
            ValidationDiff diff = new ValidationDiff();
            diff.setType("STATUS_MISMATCH");
            diff.setMessage("Esperado: " + expected.getStatus() + ", Obtido: " +
                    (hasErrors ? "error" : "success"));
            diff.setSeverity("ERROR");
            diffs.add(diff);
        }

        // Verifica mensagens de erro específicas
        if (expected.getErrors() != null) {
            expected.getErrors().forEach(expectedError -> {
                boolean found = actual.getIssue().stream()
                        .anyMatch(issue ->
                                issue.getSeverity() == OperationOutcome.IssueSeverity.ERROR &&
                                        issue.getDiagnostics() != null &&
                                        issue.getDiagnostics().contains(expectedError));

                if (!found) {
                    ValidationDiff diff = new ValidationDiff();
                    diff.setType("MISSING_ERROR");
                    diff.setMessage("Erro esperado não encontrado: " + expectedError);
                    diff.setSeverity("ERROR");
                    diffs.add(diff);
                }
            });
        }

        // Verifica warnings esperados
        if (expected.getWarnings() != null) {
            expected.getWarnings().forEach(expectedWarning -> {
                boolean found = actual.getIssue().stream()
                        .anyMatch(issue ->
                                issue.getSeverity() == OperationOutcome.IssueSeverity.WARNING &&
                                        issue.getDiagnostics() != null &&
                                        issue.getDiagnostics().contains(expectedWarning));

                if (!found) {
                    ValidationDiff diff = new ValidationDiff();
                    diff.setType("MISSING_WARNING");
                    diff.setMessage("Aviso esperado não encontrado: " + expectedWarning);
                    diff.setSeverity("WARNING");
                    diffs.add(diff);
                }
            });
        }

        result.setDifferences(diffs);
        return diffs.isEmpty() && statusMatches;
    }
}