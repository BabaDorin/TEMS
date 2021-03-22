import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageTypesPropertiesComponent } from './manage-types-properties.component';

describe('ManageTypesPropertiesComponent', () => {
  let component: ManageTypesPropertiesComponent;
  let fixture: ComponentFixture<ManageTypesPropertiesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ManageTypesPropertiesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ManageTypesPropertiesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
