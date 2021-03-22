import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageDefinitionsComponent } from './manage-definitions.component';

describe('ManageDefinitionsComponent', () => {
  let component: ManageDefinitionsComponent;
  let fixture: ComponentFixture<ManageDefinitionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ManageDefinitionsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ManageDefinitionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
