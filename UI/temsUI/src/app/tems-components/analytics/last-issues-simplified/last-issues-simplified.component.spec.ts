import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LastIssuesSimplifiedComponent } from './last-issues-simplified.component';

describe('LastIssuesSimplifiedComponent', () => {
  let component: LastIssuesSimplifiedComponent;
  let fixture: ComponentFixture<LastIssuesSimplifiedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LastIssuesSimplifiedComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LastIssuesSimplifiedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
