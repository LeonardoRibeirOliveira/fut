package fhirut;

import picocli.CommandLine;

/**
 * Classe principal responsável por iniciar a execução do programa.
 * Utiliza a biblioteca picocli para interpretar os argumentos da linha de comando
 * e delegar a execução para a classe {@link FhirutCommand}.
 */
public class Main {

    /**
     * Método principal da aplicação.
     * Interpreta os argumentos da linha de comando e executa o comando configurado.
     *
     * @param args Argumentos passados na linha de comando.
     */
    public static void main(String[] args) {
        // Executa o comando usando a biblioteca picocli
        int exitCode = new CommandLine(new FhirutCommand()).execute(args);

        // Finaliza a aplicação com o código de saída apropriado
        System.exit(exitCode);
    }
}
