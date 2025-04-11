import java.io.BufferedReader;
import java.io.File;
import java.io.InputStreamReader;

public class Main {

    public static void main(String[] args) {
        try {
            // Caminho do arquivo JAR do validator_cli
            String validatorJarPath = "validator_cli.jar"; // Atualize para o caminho correto
            String inputFile = "input.json";
            String outputFile = "output.json";

            // Comando para executar a validação
            ProcessBuilder processBuilder = new ProcessBuilder(
                    "java", "-jar", validatorJarPath,
                    "-version", "4.0.1", "-output", outputFile, inputFile
            );

            // Define o diretório de trabalho (se necessário)
            processBuilder.directory(new File(System.getProperty("user.dir")));

            // Redireciona saída de erro para a saída normal
            processBuilder.redirectErrorStream(true);

            // Executa o comando
            Process process = processBuilder.start();

            // Captura e exibe a saída do processo
            BufferedReader reader = new BufferedReader(new InputStreamReader(process.getInputStream()));
            String line;
            while ((line = reader.readLine()) != null) {
                System.out.println(line);
            }

            // Aguarda a finalização do processo
            int exitCode = process.waitFor();
            System.out.println("Processo finalizado com código: " + exitCode);

        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}
