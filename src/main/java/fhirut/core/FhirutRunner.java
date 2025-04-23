package fhirut.core;

import fhirut.exceptions.*;
import fhirut.model.TestDefinition;
import fhirut.model.TestResult;
import fhirut.model.ValidationDiff;
import fhirut.parser.YamlParser;
import fhirut.validator.FhirValidator;
import fhirut.report.HtmlReportGenerator;
import org.hl7.fhir.r4.model.OperationOutcome;

import java.io.File;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;
import java.util.stream.Collectors;
import java.util.stream.Stream;

public class FhirutRunner {
    private final FhirValidator validator;
    private final YamlParser yamlParser;
    private final HtmlReportGenerator reportGenerator;

    public FhirutRunner() {
        this.validator = new FhirValidator();
        this.yamlParser = new YamlParser();
        this.reportGenerator = new HtmlReportGenerator();
    }

    public List<TestResult> runAllTests(File directory) throws FhirutException {
        // Verifica se é um diretório de definições ou o diretório raiz
        File testDir = new File(directory, "test-definitions");
        if (!testDir.exists()) {
            testDir = directory;
        }

        try {
            List<File> testFiles = findTestFiles(directory);
            if (testFiles.isEmpty()) {
                throw new FhirutException("Nenhum arquivo de teste encontrado em: " + directory.getPath());
            }
            return runTests(testFiles);
        } catch (IOException e) {
            throw new FhirutException("Erro ao buscar arquivos de teste", e);
        }
    }

    public List<TestResult> runTests(List<File> testFiles) {
        List<TestResult> results = new ArrayList<>();

        for (File testFile : testFiles) {
            try {
                TestDefinition definition = yamlParser.parseTestDefinition(testFile);
                TestResult result = validator.validate(definition);
                results.add(result);

            } catch (FhirutParseException e) {
                results.add(createFailedResult(testFile, e));
            } catch (FhirValidationException e) {
                results.add(createFailedResult(testFile, e));
            } catch (Exception e) {
                results.add(createFailedResult(testFile,
                        new FhirutException("Erro inesperado: " + e.getMessage(), e)));
            }
        }

        return results;
    }

    /**
     * Gera relatório HTML com os resultados dos testes
     * @param results Lista de resultados a serem incluídos no relatório
     */
    public void generateReport(List<TestResult> results, File outputFile) {
        try {
            reportGenerator.generateReport(results, outputFile);
        } catch (IOException e) {
            System.err.println("Erro ao gerar relatório: " + e.getMessage());
        }
    }

    private List<File> findTestFiles(File directory) throws IOException {
        try (Stream<Path> paths = Files.walk(Paths.get(directory.getAbsolutePath()))) {
            return paths
                    .filter(Files::isRegularFile)
                    .filter(path -> path.toString().endsWith(".yml") || path.toString().endsWith(".yaml"))
                    .map(Path::toFile)
                    .collect(Collectors.toList());
        }
    }

    /**
     * Cria um resultado de teste com falha baseado em uma exceção
     * @param testFile Arquivo de teste que falhou
     * @param exception Exceção ocorrida
     * @return TestResult configurado com os detalhes do erro
     */
    private TestResult createFailedResult(File testFile, FhirutException exception) {
        TestResult result = new TestResult();
        result.setTestId(testFile.getName());
        result.setPassed(false);

        // Cria diferença de validação para o erro
        ValidationDiff diff = new ValidationDiff();
        diff.setType("EXECUTION_ERROR");
        diff.setMessage(exception.getMessage());
        diff.setSeverity("FATAL");
        diff.setLocation(testFile.getPath());

        result.setDifferences(List.of(diff));

        // Se for uma exceção de validação FHIR, preserva o OperationOutcome
        if (exception instanceof FhirValidationException fhirEx && fhirEx.getOperationOutcome() != null) {
            result.setOutcome(fhirEx.getOperationOutcome());
        } else {
            // Cria um OperationOutcome básico para erros genéricos
            OperationOutcome oo = new OperationOutcome();
            oo.addIssue()
                    .setSeverity(OperationOutcome.IssueSeverity.FATAL)
                    .setCode(OperationOutcome.IssueType.EXCEPTION)
                    .setDiagnostics(exception.getMessage());
            result.setOutcome(oo);
        }

        return result;
    }
}