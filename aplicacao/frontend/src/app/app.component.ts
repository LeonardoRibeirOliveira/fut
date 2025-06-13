import { Component } from '@angular/core';
import { TestCaseDefinition } from './models/test-case-definition';
import { TestCaseResult } from './models/test-case-result';
import { TestCaseService } from './services/test-case.service';
import { FileUploadComponent } from "./components/file-upload/file-upload.component";
import { ResultsDisplayComponent } from "./components/results-display/results-display.component";
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  imports: [FileUploadComponent, ResultsDisplayComponent, CommonModule]
})
export class AppComponent {
  testResults: TestCaseResult[] = [];
  isLoading = false;
  error: string | null = null;

  constructor(private testCaseService: TestCaseService) {}

  onFilesSelected(testCases: TestCaseDefinition[]): void {
    this.isLoading = true;
    this.error = null;

    this.testCaseService.runTestCases(testCases).subscribe({
      next: (results) => {
        this.testResults = results;
        this.isLoading = false;
      },
      error: (err) => {
        this.error = 'An error occurred while running the tests. Please try again.';
        this.isLoading = false;
        console.error(err);
      }
    });
  }
}