import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { TestCaseService } from './services/test-case.service';

@NgModule({
  declarations: [
    // Seus componentes aqui
  ],
  imports: [
    CommonModule,
    HttpClientModule
  ],
  providers: [
    TestCaseService
  ]
})
export class TestCaseModule { }