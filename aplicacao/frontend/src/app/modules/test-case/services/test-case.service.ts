import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { TestCaseDefinition } from '../models/test-case.request.model';
import { OperationOutcome } from '../models/operation-outcome.model';

@Injectable({
  providedIn: 'root',
})
export class TestCaseService {
  // A URL da API é configurada no arquivo environment.ts
  // Certifique-se de que a porta corresponde à do seu backend (ex: http://localhost:53648)
  private readonly apiUrl = `${environment.apiUrl}/api/TestCase`;

  constructor(private http: HttpClient) { }

  /**
   * Executa um ou mais casos de teste no backend.
   * @param testCases - Um array de definições de casos de teste, cada um com o conteúdo do YAML e dos JSONs.
   * @returns Um Observable com os resultados da validação (um array de OperationOutcome).
   */
  runTestCases(
    testCases: TestCaseDefinition[],
  ): Observable<OperationOutcome[][]> {
    return this.http.post<OperationOutcome[][]>(`${this.apiUrl}/run`, testCases);
  }
}