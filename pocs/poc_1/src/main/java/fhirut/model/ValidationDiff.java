package fhirut.model;

public class ValidationDiff {
    private String type; // "MISSING_ERROR", "UNEXPECTED_ERROR", etc.
    private String message;
    private String location;
    private String severity;

    // Getters e Setters


    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }

    public String getMessage() {
        return message;
    }

    public void setMessage(String message) {
        this.message = message;
    }

    public String getLocation() {
        return location;
    }

    public void setLocation(String location) {
        this.location = location;
    }

    public String getSeverity() {
        return severity;
    }

    public void setSeverity(String severity) {
        this.severity = severity;
    }
}
