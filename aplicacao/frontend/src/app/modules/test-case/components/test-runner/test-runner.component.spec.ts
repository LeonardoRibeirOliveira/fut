import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TestRunnerComponent } from './test-runner.component';

describe('TestRunnerComponent', () => {
  let component: TestRunnerComponent;
  let fixture: ComponentFixture<TestRunnerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestRunnerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TestRunnerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
