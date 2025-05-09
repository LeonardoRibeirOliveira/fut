package fhirut.model;

import java.util.List;

public class ExpectedResults {
    private String status;
    private List<String> errors;
    private List<String> warnings;
    private List<String> informations;
    private List<InvariantCheck> invariants;

    // Getters e Setters

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    public List<String> getErrors() {
        return errors;
    }

    public void setErrors(List<String> errors) {
        this.errors = errors;
    }

    public List<String> getWarnings() {
        return warnings;
    }

    public void setWarnings(List<String> warnings) {
        this.warnings = warnings;
    }

    public List<String> getInformations() {
        return informations;
    }

    public void setInformations(List<String> informations) {
        this.informations = informations;
    }

    public List<InvariantCheck> getInvariants() {
        return invariants;
    }

    public void setInvariants(List<InvariantCheck> invariants) {
        this.invariants = invariants;
    }
}
