import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewEquipmentComponent } from './view-equipment.component';

describe('ViewEquipmentComponent', () => {
  let component: ViewEquipmentComponent;
  let fixture: ComponentFixture<ViewEquipmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewEquipmentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewEquipmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
