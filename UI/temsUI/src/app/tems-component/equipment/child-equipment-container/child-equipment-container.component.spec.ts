import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChildEquipmentContainerComponent } from './child-equipment-container.component';

describe('ChildEquipmentContainerComponent', () => {
  let component: ChildEquipmentContainerComponent;
  let fixture: ComponentFixture<ChildEquipmentContainerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChildEquipmentContainerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChildEquipmentContainerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
