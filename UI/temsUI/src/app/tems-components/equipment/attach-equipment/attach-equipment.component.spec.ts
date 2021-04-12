import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AttachEquipmentComponent } from './attach-equipment.component';

describe('AttachEquipmentComponent', () => {
  let component: AttachEquipmentComponent;
  let fixture: ComponentFixture<AttachEquipmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AttachEquipmentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AttachEquipmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
