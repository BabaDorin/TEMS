import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GenericContainerComponent } from './generic-container.component';

describe('GenericContainerComponent', () => {
  let component: GenericContainerComponent;
  let fixture: ComponentFixture<GenericContainerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GenericContainerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GenericContainerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
