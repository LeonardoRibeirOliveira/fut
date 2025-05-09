package fhirut.exceptions;

import org.hl7.fhir.r4.model.OperationOutcome;
import org.hl7.fhir.r4.formats.JsonParser;
import org.hl7.fhir.r4.formats.IParser;

import java.util.Objects;

/**
 * Exceção lançada quando ocorrem erros durante a validação FHIR,
 * incluindo problemas com o validador externo ou resultados inesperados.
 */
public class FhirValidationException extends FhirutException {
    private OperationOutcome operationOutcome;
    private String validatorOutput;
    private String testId;

    public FhirValidationException(String message, String validatorOutput, String testId) {
        super(message);
        this.validatorOutput = validatorOutput;
        this.testId = testId;
    }

    /**
     * Constrói uma nova exceção com mensagem padrão.
     */
    public FhirValidationException() {
        super("Erro durante a validação FHIR");
    }

    /**
     * Constrói uma nova exceção com a mensagem especificada.
     * @param message a mensagem detalhando o erro de validação
     */
    public FhirValidationException(String message) {
        super(Objects.requireNonNull(message));
    }

    /**
     * Constrói uma nova exceção com a causa especificada.
     * @param cause a causa raiz do erro de validação
     */
    public FhirValidationException(Throwable cause) {
        super(Objects.requireNonNull(cause));
    }

    /**
     * Constrói uma nova exceção com a mensagem e causa especificadas.
     * @param message a mensagem detalhando o erro de validação
     * @param cause a causa raiz do erro de validação
     */
    public FhirValidationException(String message, Throwable cause) {
        super(Objects.requireNonNull(message), Objects.requireNonNull(cause));
    }

    /**
     * Constrói uma nova exceção com o OperationOutcome resultante da validação.
     * @param operationOutcome o resultado da validação FHIR
     */
    public FhirValidationException(OperationOutcome operationOutcome) {
        super("Falha na validação FHIR: " + operationOutcomeToString(operationOutcome));
        this.operationOutcome = Objects.requireNonNull(operationOutcome);
    }

    /**
     * Constrói uma nova exceção com mensagem e OperationOutcome.
     * @param message a mensagem detalhando o erro
     * @param operationOutcome o resultado da validação FHIR
     */
    public FhirValidationException(String message, OperationOutcome operationOutcome) {
        super(Objects.requireNonNull(message));
        this.operationOutcome = Objects.requireNonNull(operationOutcome);
    }

    /**
     * Constrói uma nova exceção com a saída bruta do validador.
     * @param message a mensagem de erro
     * @param validatorOutput a saída bruta do validador FHIR
     */
    public FhirValidationException(String message, String validatorOutput) {
        super(Objects.requireNonNull(message));
        this.validatorOutput = validatorOutput;
        try {
            this.operationOutcome = parseOperationOutcome(validatorOutput);
        } catch (Exception e) {
            // Ignora se não conseguir parsear
        }
    }

    /**
     * Constrói uma nova exceção completa.
     * @param message a mensagem de erro
     * @param cause a causa raiz
     * @param validatorOutput a saída bruta do validador
     * @param testId o ID do teste que falhou
     */
    public FhirValidationException(String message, Throwable cause, String validatorOutput, String testId) {
        super(message, cause);
        this.validatorOutput = validatorOutput;
        this.testId = testId;
        try {
            this.operationOutcome = parseOperationOutcome(validatorOutput);
        } catch (Exception e) {
            // Ignora se não conseguir parsear
        }
    }

    /**
     * Obtém o OperationOutcome associado à exceção.
     * @return o OperationOutcome ou null se não estiver disponível
     */
    public OperationOutcome getOperationOutcome() {
        return operationOutcome;
    }

    /**
     * Obtém a saída bruta do validador FHIR.
     * @return a saída do validador ou null se não estiver disponível
     */
    public String getValidatorOutput() {
        return validatorOutput;
    }

    /**
     * Obtém o ID do teste que falhou.
     * @return o ID do teste ou null se não estiver disponível
     */
    public String getTestId() {
        return testId;
    }

    private static String operationOutcomeToString(OperationOutcome outcome) {
        if (outcome == null) {
            return "Sem detalhes de validação";
        }

        StringBuilder sb = new StringBuilder();
        if (outcome.hasIssue()) {
            outcome.getIssue().forEach(issue -> {
                sb.append("[")
                        .append(issue.getSeverity() != null ? issue.getSeverity().getDisplay() : "NO_SEVERITY")
                        .append("] ")
                        .append(issue.getDetails() != null ? issue.getDetails().getText() : "No details")
                        .append(" (")
                        .append(issue.hasExpression() ? issue.getExpression().get(0).getValue() : "no location")
                        .append(")\n");
            });
        }
        return sb.toString();
    }

    private OperationOutcome parseOperationOutcome(String json) throws Exception {
        if (json == null || json.trim().isEmpty()) {
            return null;
        }
        IParser parser = new JsonParser();
        return (OperationOutcome) parser.parse(json);
    }

    @Override
    public String getMessage() {
        String baseMessage = super.getMessage();
        StringBuilder fullMessage = new StringBuilder(baseMessage);

        if (testId != null) {
            fullMessage.append("\nTest ID: ").append(testId);
        }

        if (operationOutcome != null) {
            fullMessage.append("\nDetalhes da validação:\n")
                    .append(operationOutcomeToString(operationOutcome));
        }

        if (validatorOutput != null && operationOutcome == null) {
            fullMessage.append("\nSaída bruta do validador:\n")
                    .append(validatorOutput);
        }

        return fullMessage.toString();
    }
}