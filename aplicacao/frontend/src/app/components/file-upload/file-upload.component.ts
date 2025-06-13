import { Component, EventEmitter, Output } from '@angular/core';
import { TestCaseDefinition } from '../../models/test-case-definition';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.scss'],
  imports: [CommonModule]
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
      const testCases = this.selectedFiles.map(file => ({
        yamlFilePath: this.getRelativePath(file),
      }));
      this.filesSelected.emit(testCases);
    }
  }

  clearSelection(): void {
    this.selectedFiles = [];
    window.location.reload();
  }

  private getRelativePath(file: File): string {
    if ('webkitRelativePath' in file) {
      return (file as any).webkitRelativePath || file.name;
    }

    return '';
  }
}