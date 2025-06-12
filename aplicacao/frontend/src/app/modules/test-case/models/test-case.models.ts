export interface TestCaseFileModel {
  id: string;
  name: string;
  description: string;
  userId: string;
  yamlPath?: string;
  jsonPath?: string;
  createdAt: Date;
}

export interface ValidationResultModel {
  testCaseId: string;
  executionTime: Date;
  status: 'success' | 'error' | 'warning';
  issues: ValidationIssue[];
  rawOperationOutcome: string;
}

export interface ValidationIssue {
  severity: 'error' | 'warning' | 'information';
  message: string;
  location?: string;
}

export interface BatchRunRequest {
  userId: string;
  testCaseIds: string[];
}

export interface BatchRunResponse {
  userId: string;
  results: ValidationResultModel[];
  totalCount: number;
  successCount: number;
  errorCount: number;
}

export interface UploadResponse {
  success: boolean;
  message?: string;
  testCases?: TestCaseFileModel[];
}

export interface TestCaseListResponse {
  testCases: TestCaseFileModel[];
  totalCount: number;
}

