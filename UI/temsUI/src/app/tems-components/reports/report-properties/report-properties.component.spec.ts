import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportPropertiesComponent } from './report-properties.component';

describe('ReportPropertiesComponent', () => {
  let component: ReportPropertiesComponent;
  let fixture: ComponentFixture<ReportPropertiesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReportPropertiesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportPropertiesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
