package fhirut.validator;

public class ProcessResult {
    private final String output;
    private final int exitCode;

    public ProcessResult(String output, int exitCode) {
        this.output = output;
        this.exitCode = exitCode;
    }

    public String getOutput() {
        return output;
    }

    public int getExitCode() {
        return exitCode;
    }

    public boolean isSuccess() {
        return exitCode == 0;
    }
}