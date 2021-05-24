import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BooleanCellRendererComponent } from './boolean-cell-renderer.component';

describe('BooleanCellRendererComponent', () => {
  let component: BooleanCellRendererComponent;
  let fixture: ComponentFixture<BooleanCellRendererComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BooleanCellRendererComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BooleanCellRendererComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
