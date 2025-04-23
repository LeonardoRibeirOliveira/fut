package fhirut.exceptions;

/**
 * Exceção lançada quando ocorrem erros durante o parsing de arquivos
 * de definição de teste (YAML/JSON) ou recursos FHIR.
 */
public class FhirutParseException extends FhirutException {

    /**
     * Constrói uma nova exceção com mensagem padrão.
     */
    public FhirutParseException() {
        super("Erro durante o parsing de arquivo");
    }

    /**
     * Constrói uma nova exceção com a mensagem especificada.
     * @param message a mensagem detalhando o erro de parsing
     */
    public FhirutParseException(String message) {
        super(message);
    }

    /**
     * Constrói uma nova exceção com a causa especificada.
     * @param cause a causa raiz do erro de parsing
     */
    public FhirutParseException(Throwable cause) {
        super(cause);
    }

    /**
     * Constrói uma nova exceção com a mensagem e causa especificadas.
     * @param message a mensagem detalhando o erro de parsing
     * @param cause a causa raiz do erro de parsing
     */
    public FhirutParseException(String message, Throwable cause) {
        super(message, cause);
    }

    /**
     * Constrói uma nova exceção com informações adicionais sobre o arquivo.
     * @param message a mensagem detalhando o erro
     * @param filePath caminho do arquivo que causou o erro
     * @param lineNumber número da linha onde ocorreu o erro (opcional)
     */
    public FhirutParseException(String message, String filePath, Integer lineNumber) {
        super(formatMessage(message, filePath, lineNumber));
    }

    private static String formatMessage(String message, String filePath, Integer lineNumber) {
        StringBuilder sb = new StringBuilder(message);
        sb.append(" [Arquivo: ").append(filePath);
        if (lineNumber != null) {
            sb.append(", Linha: ").append(lineNumber);
        }
        sb.append("]");
        return sb.toString();
    }
}