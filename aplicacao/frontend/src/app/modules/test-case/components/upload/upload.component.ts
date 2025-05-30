import { Component } from '@angular/core';
import { TestCaseService } from '../../services/test-case.service';
import { UploadResponse } from '../../models/test-case.models';

@Component({
  selector: 'app-test-case-upload',
  templateUrl: './upload.component.html',
})
export class TestCaseUploadComponent {
  userId = '';
  description = '';
  yamlFiles: File[] = [];
  jsonFiles: File[] = [];
  uploadResponse?: UploadResponse;

  constructor(private testCaseService: TestCaseService) {}

  onYamlFilesSelected(event: any): void {
    this.yamlFiles = Array.from(event.target.files);
  }

  onJsonFilesSelected(event: any): void {
    this.jsonFiles = Array.from(event.target.files);
  }

  uploadFiles(): void {
    this.testCaseService
      .uploadTestCasesBatch(
        this.userId,
        this.description,
        this.yamlFiles,
        this.jsonFiles,
      )
      .subscribe({
        next: (response) => {
          this.uploadResponse = response;
        },
        error: (err) => {
          this.uploadResponse = {
            success: false,
            message: err.error.message || 'Upload failed',
          };
        },
      });
  }
}
