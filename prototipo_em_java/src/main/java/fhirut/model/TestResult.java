package fhirut.model;

import org.hl7.fhir.r4.model.OperationOutcome;

import java.util.List;

public class TestResult {
    private String testId;
    private boolean passed;
    private OperationOutcome outcome;
    private List<ValidationDiff> differences;

    // Getters e Setters

    public String getTestId() {
        return testId;
    }

    public void setTestId(String testId) {
        this.testId = testId;
    }

    public boolean isPassed() {
        return passed;
    }

    public void setPassed(boolean passed) {
        this.passed = passed;
    }

    public OperationOutcome getOutcome() {
        return outcome;
    }

    public void setOutcome(OperationOutcome outcome) {
        this.outcome = outcome;
    }

    public List<ValidationDiff> getDifferences() {
        return differences;
    }

    public void setDifferences(List<ValidationDiff> differences) {
        this.differences = differences;
    }
}

