import { Component, OnInit } from '@angular/core';
import { TestCaseService } from '../../services/test-case.service';
import {
  TestCaseFileModel,
  TestCaseListResponse,
} from '../../models/test-case.models';

@Component({
  selector: 'app-test-case-list',
  templateUrl: './list.component.html',
})
export class TestCaseListComponent implements OnInit {
  testCases: TestCaseFileModel[] = [];
  userId = '';

  constructor(private testCaseService: TestCaseService) {}

  ngOnInit(): void {
    this.loadTestCases();
  }

  loadTestCases(): void {
    if (!this.userId) return;

    this.testCaseService.getTestCasesBatch(this.userId).subscribe({
      next: (response) => {
        this.testCases = response.testCases;
      },
      error: (err) => {
        console.error('Failed to load test cases', err);
      },
    });
  }

  runBatch(): void {
    const request = {
      userId: this.userId,
      testCaseIds: this.testCases.map((tc) => tc.id),
    };

    this.testCaseService.runTestCasesBatch(request).subscribe({
      next: (response) => {
        console.log('Batch execution results:', response);
      },
      error: (err) => {
        console.error('Batch execution failed', err);
      },
    });
  }
}
