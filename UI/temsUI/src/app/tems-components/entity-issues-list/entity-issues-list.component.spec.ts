import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EntityIssuesListComponent } from './entity-issues-list.component';

describe('EntityIssuesListComponent', () => {
  let component: EntityIssuesListComponent;
  let fixture: ComponentFixture<EntityIssuesListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EntityIssuesListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EntityIssuesListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
