import { OperationOutcome } from "fhir/r4";

export interface TestCaseResult {
  testId: string;
  jsonId: string;
  expectedStatus: string;
  actualStatus: string;
  issues: IssueSummary[];
  executionTime: string;
  operationOutcome?: OperationOutcome;
}

export interface IssueSummary {
  severity: string;
  location: string;
  code: string;
  details: string;
  source: string;
}
