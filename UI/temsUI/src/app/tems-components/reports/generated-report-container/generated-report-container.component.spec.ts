import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GeneratedReportContainerComponent } from './generated-report-container.component';

describe('GeneratedReportContainerComponent', () => {
  let component: GeneratedReportContainerComponent;
  let fixture: ComponentFixture<GeneratedReportContainerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GeneratedReportContainerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GeneratedReportContainerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
