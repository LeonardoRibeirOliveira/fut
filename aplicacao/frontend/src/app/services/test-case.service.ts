import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TestCaseDefinition } from '../models/test-case-definition';
import { TestCaseResult } from '../models/test-case-result';

@Injectable({
  providedIn: 'root'
})
export class TestCaseService {
  private apiUrl = 'https://localhost:57119/api/TestCase/run';

  constructor(private http: HttpClient) { }

  runTestCases(testCases: TestCaseDefinition[]): Observable<TestCaseResult[]> {
    return this.http.post<TestCaseResult[]>(this.apiUrl, testCases);
  }
}