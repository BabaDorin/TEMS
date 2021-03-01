import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EntityAllocationsListComponent } from './entity-allocations-list.component';

describe('EntityAllocationsListComponent', () => {
  let component: EntityAllocationsListComponent;
  let fixture: ComponentFixture<EntityAllocationsListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EntityAllocationsListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EntityAllocationsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
