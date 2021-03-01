import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EquipmentDetailsIssuesComponent } from './equipment-details-issues.component';

describe('EquipmentDetailsIssuesComponent', () => {
  let component: EquipmentDetailsIssuesComponent;
  let fixture: ComponentFixture<EquipmentDetailsIssuesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EquipmentDetailsIssuesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EquipmentDetailsIssuesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
