import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EquipmentTypesListComponent } from './equipment-types-list.component';

describe('EquipmentTypesListComponent', () => {
  let component: EquipmentTypesListComponent;
  let fixture: ComponentFixture<EquipmentTypesListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EquipmentTypesListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EquipmentTypesListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
