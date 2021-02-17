import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EquipmentDetailsLogsComponent } from './equipment-details-logs.component';

describe('EquipmentDetailsLogsComponent', () => {
  let component: EquipmentDetailsLogsComponent;
  let fixture: ComponentFixture<EquipmentDetailsLogsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EquipmentDetailsLogsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EquipmentDetailsLogsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
