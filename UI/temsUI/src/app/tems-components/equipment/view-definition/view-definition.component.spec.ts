import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewDefinitionComponent } from './view-definition.component';

describe('ViewDefinitionComponent', () => {
  let component: ViewDefinitionComponent;
  let fixture: ComponentFixture<ViewDefinitionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewDefinitionComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewDefinitionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
