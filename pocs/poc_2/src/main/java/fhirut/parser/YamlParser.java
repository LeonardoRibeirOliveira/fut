package fhirut.parser;

import com.fasterxml.jackson.databind.DeserializationFeature;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.PropertyNamingStrategies;
import com.fasterxml.jackson.dataformat.yaml.YAMLFactory;
import fhirut.exceptions.FhirutParseException;
import fhirut.model.TestDefinition;

import java.io.File;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;

public class YamlParser {
    private final ObjectMapper yamlMapper;

    public YamlParser() {
        this.yamlMapper = new ObjectMapper(new YAMLFactory());

        // Configurações para lidar com a diferença de nomenclatura
        this.yamlMapper.setPropertyNamingStrategy(PropertyNamingStrategies.SNAKE_CASE); // Converte snake_case para camelCase
        this.yamlMapper.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, false);
        this.yamlMapper.findAndRegisterModules();
    }

    public TestDefinition parseTestDefinition(File yamlFile) throws FhirutParseException {
        try {
            // 1. Verificação básica do arquivo
            if (!yamlFile.exists()) {
                throw new FhirutParseException("Arquivo YAML não encontrado: " + yamlFile.getAbsolutePath());
            }

            if (yamlFile.length() == 0) {
                throw new FhirutParseException("Arquivo YAML está vazio");
            }

            // 2. Parse do conteúdo
            TestDefinition definition = yamlMapper.readValue(yamlFile, TestDefinition.class);

            // 3. Validação básica
            if (definition.getTestId() == null || definition.getTestId().isEmpty()) {
                throw new FhirutParseException("O campo 'test_id' é obrigatório no YAML");
            }

            if (definition.getInstancePath() == null) {
                throw new FhirutParseException("O campo 'instance_path' é obrigatório no YAML");
            }

            // 4. Conversão de caminhos relativos para absolutos
            resolvePaths(definition, yamlFile);

            return definition;

        } catch (IOException e) {
            throw new FhirutParseException("Falha ao analisar definição de teste YAML: " + e.getMessage(), e);
        }
    }

    private void resolvePaths(TestDefinition definition, File yamlFile) {
        // 1. Determina o diretório base correto (target/test-classes ou src/test/resources)
        Path baseDir = getCorrectBaseDirectory(yamlFile);

        // 2. Resolve instance_path
        if (definition.getInstancePath() != null) {
            String relativePath = definition.getInstancePath().getPath()
                    .replace("\\", "/")
                    .replace("src/test/resources/", "")
                    .replace("src\\test\\resources\\", "");

            Path resolvedPath = baseDir.resolve(relativePath).normalize();
            definition.setInstancePath(resolvedPath.toFile());

            System.out.println("[DEBUG] instance_path resolvido: " + resolvedPath);
        }

        // 3. Resolve resources no contexto
        if (definition.getContext() != null && definition.getContext().getResources() != null) {
            definition.getContext().getResources().replaceAll(file -> {
                String relativePath = file.getPath()
                        .replace("\\", "/")
                        .replace("src/test/resources/", "")
                        .replace("src\\test\\resources\\", "");

                Path resolvedPath = baseDir.resolve(relativePath).normalize();
                System.out.println("[DEBUG] resource resolvido: " + resolvedPath);
                return resolvedPath.toFile();
            });
        }
    }

    private Path getCorrectBaseDirectory(File yamlFile) {
        // Verifica se estamos executando do target/test-classes
        Path yamlPath = yamlFile.toPath().normalize();
        Path targetPath = Paths.get("target/test-classes").normalize();

        if (yamlPath.startsWith(targetPath)) {
            // Se o YAML está no target, usamos target como base
            return targetPath;
        } else {
            // Caso contrário, usamos src/test/resources
            return Paths.get("src/test/resources").normalize();
        }
    }

}