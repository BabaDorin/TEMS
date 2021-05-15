import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PinnedIssueComponent } from './pinned-issue.component';

describe('PinnedIssueComponent', () => {
  let component: PinnedIssueComponent;
  let fixture: ComponentFixture<PinnedIssueComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PinnedIssueComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PinnedIssueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
