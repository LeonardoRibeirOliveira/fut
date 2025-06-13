import { Component, EventEmitter, Output } from '@angular/core';
import { TestCaseDefinition } from '../../models/test-case-definition';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.scss'],
  imports: [CommonModule],
  standalone: true,
})
export class FileUploadComponent {
  @Output() filesSelected = new EventEmitter<TestCaseDefinition[]>();
  selectedFiles: File[] = [];

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      this.selectedFiles = Array.from(input.files);
    }
  }

  uploadFiles(): void {
    if (this.selectedFiles.length > 0) {
      const readPromises = this.selectedFiles.map(file => this.readFileAsText(file));
      
      Promise.all(readPromises).then(contents => {
        const testCases: TestCaseDefinition[] = contents.map(content => ({
          yamlFile: content
        }));
        this.filesSelected.emit(testCases);
      });
    }
  }

  clearSelection(): void {
    this.selectedFiles = [];
    window.location.reload();
  }

  private readFileAsText(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => resolve(reader.result as string);
      reader.onerror = error => reject(error);
      reader.readAsText(file);
    });
  }
}
