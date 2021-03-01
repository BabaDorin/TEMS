import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AutocompleteTypeComponent } from './autocomplete-type.component';

describe('AutocompleteTypeComponent', () => {
  let component: AutocompleteTypeComponent;
  let fixture: ComponentFixture<AutocompleteTypeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AutocompleteTypeComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AutocompleteTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
