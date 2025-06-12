export interface OperationOutcome {
    resourceType: 'OperationOutcome';
    issue: OperationOutcomeIssue[];
}

export interface OperationOutcomeIssue {
    severity: 'fatal' | 'error' | 'warning' | 'information';
    code: string;
    details?: {
        text: string;
    };
    expression?: string[];
}