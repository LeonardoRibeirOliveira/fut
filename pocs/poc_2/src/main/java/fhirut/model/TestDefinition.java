package fhirut.model;

import java.io.File;
import java.util.List;

public class TestDefinition {
    private String testId;
    private String description;
    private TestContext context;
    private File instancePath;
    private ExpectedResults expectedResults;

    // Getters e Setters
    public String getTestId() {
        return testId;
    }

    public void setTestId(String testId) {
        this.testId = testId;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public TestContext getContext() {
        return context;
    }

    public void setContext(TestContext context) {
        this.context = context;
    }

    public File getInstancePath() {
        return instancePath;
    }

    public void setInstancePath(File instancePath) {
        this.instancePath = instancePath;
    }

    public ExpectedResults getExpectedResults() {
        return expectedResults;
    }

    public void setExpectedResults(ExpectedResults expectedResults) {
        this.expectedResults = expectedResults;
    }
}

class InvariantCheck {
    private String expression;
    private boolean expected;

    // Getters e Setters

    public String getExpression() {
        return expression;
    }

    public void setExpression(String expression) {
        this.expression = expression;
    }

    public boolean isExpected() {
        return expected;
    }

    public void setExpected(boolean expected) {
        this.expected = expected;
    }
}