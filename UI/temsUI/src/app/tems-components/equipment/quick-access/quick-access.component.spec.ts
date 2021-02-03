import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuickAccessComponent } from './quick-access.component';

describe('QuickAccessComponent', () => {
  let component: QuickAccessComponent;
  let fixture: ComponentFixture<QuickAccessComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ QuickAccessComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(QuickAccessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
