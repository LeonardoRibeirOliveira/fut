package fhirut;

import picocli.CommandLine.Command;
import picocli.CommandLine.Parameters;
import picocli.CommandLine.Option;
import fhirut.core.FhirutRunner;
import fhirut.model.*;

import java.awt.*;
import java.io.File;
import java.io.IOException;
import java.util.List;
import java.util.concurrent.Callable;

@Command(
        name = "fut",
        mixinStandardHelpOptions = true,
        version = "fut 1.0",
        description = "FHIR Unit Test Tool"
)
public class FhirutCommand implements Callable<Integer> {

    @Parameters(index = "0..*", description = "Test files or patterns to run")
    private List<File> testFiles;

    @Option(names = {"-o", "--output"}, description = "Output directory for reports")
    private File outputDir;

    @Option(names = {"-v", "--verbose"}, description = "Verbose output")
    private boolean verbose;

    @Override
    public Integer call() throws Exception {
        FhirutRunner runner = new FhirutRunner();
        List<TestResult> results;

        if (testFiles == null || testFiles.isEmpty()) {
            System.out.println("🔍 Procurando testes no diretório atual...");
            results = runner.runAllTests(new File("."));
        } else {
            System.out.println("🔍 Executando testes especificados...");
            results = runner.runTests(testFiles);
        }

        File reportDir = outputDir != null ? outputDir : new File(".");
        if (!reportDir.exists()) {
            reportDir.mkdirs();
        }

        File reportFile = new File(reportDir, "fhir-test-report.html");
        System.out.println("📊 Gerando relatório em: " + reportFile.getAbsolutePath());

        runner.generateReport(results, reportFile);
        openHtmlReport(reportFile);

        long passed = results.stream().filter(TestResult::isPassed).count();
        System.out.println("✅ " + passed + " testes aprovados");
        System.out.println("❌ " + (results.size() - passed) + " testes falhos");

        return 0;
    }

    // Método corrigido - removido 'class' e adicionado 'void'
    private void openHtmlReport(File htmlFile) {
        try {
            // Normaliza o caminho para remover ./ ou .\
            File normalizedFile = htmlFile.getAbsoluteFile();
            String cleanPath = normalizedFile.getAbsolutePath();

            System.out.println("🔍 Verificando arquivo em: " + cleanPath); // Log para debug

            // Verifica se o arquivo existe
            if (!normalizedFile.exists()) {
                System.err.println("❌ Arquivo de relatório não encontrado: " + cleanPath);
                return;
            }

            // Tenta abrir com Desktop API
            if (Desktop.isDesktopSupported()) {
                Desktop desktop = Desktop.getDesktop();
                if (desktop.isSupported(Desktop.Action.BROWSE)) {
                    desktop.open(normalizedFile);
                    return;
                }
            }

            // Fallback para comandos do sistema operacional
            String os = System.getProperty("os.name").toLowerCase();
            try {
                if (os.contains("win")) {
                    // Comando corrigido para Windows - usando array para evitar problemas com espaços
                    Runtime.getRuntime().exec(new String[]{"cmd", "/c", "start", "", cleanPath});
                } else if (os.contains("mac")) {
                    Runtime.getRuntime().exec(new String[]{"open", cleanPath});
                } else {
                    Runtime.getRuntime().exec(new String[]{"xdg-open", cleanPath});
                }
            } catch (IOException e) {
                System.err.println("❌ Falha ao executar comando do sistema: " + e.getMessage());
            }

            System.out.println("📌 Acesse manualmente: " + cleanPath);

        } catch (Exception e) {
            System.err.println("❌ Erro crítico ao abrir relatório: " + e.getMessage());
        }
    }
}