import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IssueContainerSimplifiedComponent } from './issue-container-simplified.component';

describe('IssueContainerSimplifiedComponent', () => {
  let component: IssueContainerSimplifiedComponent;
  let fixture: ComponentFixture<IssueContainerSimplifiedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ IssueContainerSimplifiedComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(IssueContainerSimplifiedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
