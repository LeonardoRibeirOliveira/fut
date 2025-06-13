import { OperationOutcome } from "fhir/r4";

export interface TestCaseResult {
  testId: string;
  yamlId: string;
  expectedStatus: string;
  actualStatus: string;
  issues: IssueSummary[];
  executionTime: TimeSpan;
  operationOutcome?: OperationOutcome;
}

export interface IssueSummary {
  severity: string;
  location: string;
  code: string;
  details: string;
  source: string;
}

export interface TimeSpan {
  ticks: number;
  days: number;
  hours: number;
  milliseconds: number;
  minutes: number;
  seconds: number;
  totalDays: number;
  totalHours: number;
  totalMilliseconds: number;
  totalMinutes: number;
  totalSeconds: number;
}