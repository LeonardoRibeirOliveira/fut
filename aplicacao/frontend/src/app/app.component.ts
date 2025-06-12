import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TestRunnerComponent } from './modules/test-case/components/test-runner/test-runner.component';

@Component({
  selector: 'app-root',
  standalone: true,
  // Importe o TestRunnerComponent aqui
  imports: [RouterOutlet, TestRunnerComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'FHIR Unit Test';
}