package Fhir;

import java.util.List;

public class TestCase {
    private String testId;
    private String description;
    private TestContext context;
    private String instancePath;
    private ExpectedResults expectedResults;

    // Getters e Setters
    public static class TestContext {
        private List<String> igs;
        private List<String> profiles;
        private List<String> resources;
        // Getters e Setters
    }

    public static class ExpectedResults {
        private String status;
        private List<String> errors;
        private List<String> warnings;
        private List<String> informations;
        private List<Invariant> invariants;
        // Getters e Setters
    }

    public static class Invariant {
        private String expression;
        private boolean expected;
        // Getters e Setters
    }
}
