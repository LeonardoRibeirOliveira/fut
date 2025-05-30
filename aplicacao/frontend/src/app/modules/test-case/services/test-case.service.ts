import { Injectable } from '@angular/core';
import { HttpClient, HttpEvent, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  TestCaseFileModel,
  ValidationResultModel,
  BatchRunRequest,
  BatchRunResponse,
  UploadResponse,
  TestCaseListResponse,
} from '../models/test-case.models';

@Injectable({
  providedIn: 'root',
})
export class TestCaseService {
  private readonly apiUrl = `${environment.apiUrl}/api/testcase`;

  constructor(private http: HttpClient) {}

  uploadTestCasesBatch(
    userId: string,
    description: string,
    yamlFiles: File[],
    jsonFiles?: File[],
  ): Observable<UploadResponse> {
    const formData = new FormData();
    formData.append('userId', userId);
    formData.append('description', description);

    yamlFiles.forEach((file) => formData.append('yamlFiles', file));
    if (jsonFiles) {
      jsonFiles.forEach((file) => formData.append('jsonFiles', file));
    }

    return this.http.post<UploadResponse>(
      `${this.apiUrl}/upload/batch`,
      formData,
    );
  }

  getTestCasesBatch(userId: string): Observable<TestCaseListResponse> {
    return this.http.get<TestCaseListResponse>(`${this.apiUrl}/${userId}`);
  }

  getTestCase(userId: string, caseId: string): Observable<TestCaseFileModel> {
    return this.http.get<TestCaseFileModel>(
      `${this.apiUrl}/${userId}/${caseId}`,
    );
  }

  runTestCasesBatch(request: BatchRunRequest): Observable<BatchRunResponse> {
    return this.http.post<BatchRunResponse>(
      `${this.apiUrl}/batch/run`,
      request,
    );
  }
}
