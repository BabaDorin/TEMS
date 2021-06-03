import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EquipmentDefinitionsListComponent } from './equipment-definitions-list.component';

describe('EquipmentDefinitionsListComponent', () => {
  let component: EquipmentDefinitionsListComponent;
  let fixture: ComponentFixture<EquipmentDefinitionsListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EquipmentDefinitionsListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EquipmentDefinitionsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
