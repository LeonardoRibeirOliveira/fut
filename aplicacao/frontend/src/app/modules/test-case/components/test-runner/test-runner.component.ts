import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TestCaseService } from '../../services/test-case.service';
import { TestCaseDefinition } from '../../models/test-case.request.model';
import { OperationOutcome } from '../../models/operation-outcome.model';

@Component({
  selector: 'app-test-runner',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './test-runner.component.html',
  styleUrls: ['./test-runner.component.css'],
})
export class TestRunnerComponent {
  yamlFile: File | null = null;
  jsonFiles: File[] = [];
  results: OperationOutcome[] = [];
  errorMessage: string | null = null;
  isLoading = false;

  constructor(private testCaseService: TestCaseService) { }

  // Captura o arquivo YAML selecionado
  onYamlFileSelected(event: Event): void {
    const element = event.currentTarget as HTMLInputElement;
    let fileList: FileList | null = element.files;
    if (fileList && fileList.length > 0) {
      this.yamlFile = fileList[0];
    }
  }

  // Captura os arquivos JSON selecionados
  onJsonFilesSelected(event: Event): void {
    const element = event.currentTarget as HTMLInputElement;
    let fileList: FileList | null = element.files;
    if (fileList) {
      this.jsonFiles = Array.from(fileList);
    }
  }

  // Função principal para executar os testes
  async runTests(): Promise<void> {
    if (!this.yamlFile || this.jsonFiles.length === 0) {
      this.errorMessage = 'Por favor, selecione um arquivo YAML e pelo menos um arquivo JSON.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;
    this.results = [];

    try {
      // Lê o conteúdo dos arquivos como texto
      const yamlContent = await this.readFileAsText(this.yamlFile);
      const jsonContents = await Promise.all(
        this.jsonFiles.map((file) => this.readFileAsText(file))
      );

      // Monta o payload para a API
      const testCase: TestCaseDefinition = {
        yamlFile: yamlContent,
        jsonFiles: jsonContents,
      };

      // Chama o serviço da API
      this.testCaseService.runTestCases([testCase]).subscribe({
        next: (response) => {
          // O backend retorna [[OperationOutcome], [OperationOutcome], ...], então pegamos o primeiro item
          this.results = response.length > 0 ? response.flat() : [];
          this.isLoading = false;
        },
        error: (err) => {
          this.errorMessage = `Ocorreu um erro na validação: ${err.message}`;
          console.error(err);
          this.isLoading = false;
        },
      });
    } catch (error) {
      this.errorMessage = 'Falha ao ler os arquivos.';
      console.error(error);
      this.isLoading = false;
    }
  }

  // Função auxiliar para ler um arquivo e retornar seu conteúdo como string
  private readFileAsText(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => resolve(reader.result as string);
      reader.onerror = () => reject(reader.error);
      reader.readAsText(file);
    });
  }

  // Função para obter a classe CSS com base na severidade do problema
  getSeverityClass(severity: string): string {
    switch (severity) {
      case 'error':
        return 'bg-red-100 border-red-400 text-red-700';
      case 'warning':
        return 'bg-yellow-100 border-yellow-400 text-yellow-700';
      case 'information':
        return 'bg-blue-100 border-blue-400 text-blue-700';
      default:
        return 'bg-gray-100 border-gray-400 text-gray-700';
    }
  }
}