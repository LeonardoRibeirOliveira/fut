package fhirut.report;

import fhirut.model.TestResult;
import org.thymeleaf.TemplateEngine;
import org.thymeleaf.context.Context;
import org.thymeleaf.templatemode.TemplateMode;
import org.thymeleaf.templateresolver.ClassLoaderTemplateResolver;

import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.net.URL;
import java.util.List;

public class HtmlReportGenerator {
    public void generateReport(List<TestResult> results, File outputFile) throws IOException {
        URL templateUrl = getClass().getResource("/templates/report-template.html");
        if (templateUrl == null) {
            throw new IOException("Template não encontrado! Verifique se está em src/main/resources/templates/");
        }
        TemplateEngine templateEngine = configureTemplateEngine();
        Context context = new Context();
        context.setVariable("results", results);

        try (FileWriter writer = new FileWriter(outputFile)) {
            templateEngine.process("report-template", context, writer);
        }
    }

    private TemplateEngine configureTemplateEngine() {
        ClassLoaderTemplateResolver templateResolver = new ClassLoaderTemplateResolver();
        templateResolver.setPrefix("/templates/");  // Procura em src/main/resources/templates/
        templateResolver.setSuffix(".html");
        templateResolver.setTemplateMode(TemplateMode.HTML);
        templateResolver.setCharacterEncoding("UTF-8");

        TemplateEngine templateEngine = new TemplateEngine();
        templateEngine.setTemplateResolver(templateResolver);
        return templateEngine;
    }
}