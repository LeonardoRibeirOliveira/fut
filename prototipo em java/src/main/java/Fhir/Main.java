package Fhir;

import org.yaml.snakeyaml.Yaml;
import org.yaml.snakeyaml.constructor.Constructor;
import java.io.*;
import java.util.*;

public class Main {
    public static void main(String[] args) {
        Yaml yaml = new Yaml(new Constructor(TestCase.class));
        String testFile = args.length > 0 ? args[0] : "src/main/resources/tests/patient-001.yml";

        try (InputStream in = new FileInputStream(testFile)) {
            TestCase test = yaml.load(in);
            String result = Validator.validate(test);
            System.out.println("Resultado da validação:\n" + result);
        } catch (Exception e) {
            System.err.println("Erro: " + e.getMessage());
        }
    }
}