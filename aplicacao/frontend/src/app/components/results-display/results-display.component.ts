import { Component, Input } from '@angular/core';
import { TestCaseResult } from '../../models/test-case-result';
import { CommonModule } from '@angular/common';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-results-display',
  templateUrl: './results-display.component.html',
  styleUrls: ['./results-display.component.scss'],
  imports: [CommonModule, FormsModule] 
})
export class ResultsDisplayComponent {
  @Input() results: TestCaseResult[] = [];
  expandedResult: string | null = null;
  showOperationOutcome = false;

  constructor(private sanitizer: DomSanitizer) {}

  toggleExpand(jsonId: string): void {
    this.expandedResult = this.expandedResult === jsonId ? null : jsonId;
  }

  getFileName(path: string): string {
    return path.split('\\').pop() || path;
  }

  getExpectedStatus(status: string): string {
    return status.split('.').pop() || status;
  }

  countIssuesBySeverity(issues: any[], severity: string): number {
    return issues.filter(issue => issue.severity === severity).length;
  }

  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'success':
        return 'bg-green-100 text-green-800';
      case 'error':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-yellow-100 text-yellow-800';
    }
  }

  getStatusText(status: string): string {
    switch (status.toLowerCase()) {
      case 'success':
        return '✔️ Passed';
      case 'error':
        return '❌ Failed';
      default:
        return '⚠️ ' + status;
    }
  }

  downloadOperationOutcome(operationOutcome: any, testId: string): void {
    const blob = new Blob([JSON.stringify(operationOutcome, null, 2)], { type: 'application/json' });
    
    const url = window.URL.createObjectURL(blob);
    
    const a = document.createElement('a');
    a.href = url;
    a.download = `operation-outcome-${testId}-${new Date().toISOString().slice(0, 10)}.json`;
    
    document.body.appendChild(a);
    a.click();
    
    window.URL.revokeObjectURL(url);
    document.body.removeChild(a);
  }
}