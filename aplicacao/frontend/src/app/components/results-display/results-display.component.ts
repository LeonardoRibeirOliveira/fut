import { Component, Input } from '@angular/core';
import { TestCaseResult } from '../../models/test-case-result';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-results-display',
  templateUrl: './results-display.component.html',
  styleUrls: ['./results-display.component.scss'],
  imports: [CommonModule]
})
export class ResultsDisplayComponent {
  @Input() results: TestCaseResult[] = [];
  expandedResult: string | null = null;

  toggleExpand(jsonId: string): void {
    this.expandedResult = this.expandedResult === jsonId ? null : jsonId;
  }

  formatTimeSpan(timeSpan: any): string {
    if (!timeSpan) return 'N/A';
    return `${timeSpan.seconds}.${timeSpan.milliseconds}s`;
  }
}