import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PinnedIssuesComponent } from './pinned-issues.component';

describe('PinnedIssuesComponent', () => {
  let component: PinnedIssuesComponent;
  let fixture: ComponentFixture<PinnedIssuesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PinnedIssuesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PinnedIssuesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
