package Fhir;

import java.io.*;
import java.nio.file.*;
import java.util.*;

public class Validator {
    public static String validate(TestCase test) throws Exception {
        String jarPath = "src/main/resources/validator_cli.jar";
        String outputPath = "target/output.json";

        List<String> command = new ArrayList<>();
        command.addAll(Arrays.asList("java", "-jar", jarPath));
        command.add(test.getInstancePath());
        command.add("-output");
        command.add(outputPath);

        // Adiciona perfis
        for (String profile : test.getContext().getProfiles()) {
            command.add("-profile");
            command.add(profile);
        }

        ProcessBuilder pb = new ProcessBuilder(command);
        Process process = pb.start();
        process.waitFor();

        return new String(Files.readAllBytes(Paths.get(outputPath)));
    }
}