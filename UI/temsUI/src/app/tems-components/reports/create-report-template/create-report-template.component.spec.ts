import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateReportTemplateComponent } from './create-report-template.component';

describe('CreateReportTemplateComponent', () => {
  let component: CreateReportTemplateComponent;
  let fixture: ComponentFixture<CreateReportTemplateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateReportTemplateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateReportTemplateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
