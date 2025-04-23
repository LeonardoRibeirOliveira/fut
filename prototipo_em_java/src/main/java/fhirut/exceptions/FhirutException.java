package fhirut.exceptions;

/**
 * Classe base para exceções do projeto FHIRUT.
 * Todas as exceções específicas do FHIRUT devem estender esta classe.
 */
public class FhirutException extends Exception {

    /**
     * Constrói uma nova exceção com mensagem padrão.
     */
    public FhirutException() {
        super("Ocorreu um erro no FHIR Unit Test Suite");
    }

    /**
     * Constrói uma nova exceção com a mensagem especificada.
     * @param message a mensagem detalhando o erro
     */
    public FhirutException(String message) {
        super(message);
    }

    /**
     * Constrói uma nova exceção com a causa especificada.
     * @param cause a causa raiz do erro
     */
    public FhirutException(Throwable cause) {
        super(cause);
    }

    /**
     * Constrói uma nova exceção com a mensagem e causa especificadas.
     * @param message a mensagem detalhando o erro
     * @param cause a causa raiz do erro
     */
    public FhirutException(String message, Throwable cause) {
        super(message, cause);
    }

    /**
     * Constrói uma nova exceção com parâmetros adicionais de controle.
     * @param message a mensagem detalhando o erro
     * @param cause a causa raiz do erro
     * @param enableSuppression habilitar supressão
     * @param writableStackTrace habilitar stack trace gravável
     */
    protected FhirutException(String message, Throwable cause,
                              boolean enableSuppression,
                              boolean writableStackTrace) {
        super(message, cause, enableSuppression, writableStackTrace);
    }
}