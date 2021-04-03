import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IssueContainerComponent } from './issue-container.component';

describe('IssueContainerComponent', () => {
  let component: IssueContainerComponent;
  let fixture: ComponentFixture<IssueContainerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ IssueContainerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(IssueContainerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
