import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PropertiesListComponent } from './properties-list.component';

describe('PropertiesListComponent', () => {
  let component: PropertiesListComponent;
  let fixture: ComponentFixture<PropertiesListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PropertiesListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PropertiesListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
