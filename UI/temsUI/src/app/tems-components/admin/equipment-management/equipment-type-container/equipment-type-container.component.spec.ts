import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EquipmentTypeContainerComponent } from './equipment-type-container.component';

describe('EquipmentTypeContainerComponent', () => {
  let component: EquipmentTypeContainerComponent;
  let fixture: ComponentFixture<EquipmentTypeContainerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EquipmentTypeContainerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EquipmentTypeContainerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
