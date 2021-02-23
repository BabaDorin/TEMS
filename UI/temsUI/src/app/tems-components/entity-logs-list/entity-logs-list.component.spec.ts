import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EntityLogsListComponent } from './entity-logs-list.component';

describe('EntityLogsListComponent', () => {
  let component: EntityLogsListComponent;
  let fixture: ComponentFixture<EntityLogsListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EntityLogsListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EntityLogsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
