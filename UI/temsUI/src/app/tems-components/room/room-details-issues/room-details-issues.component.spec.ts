import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoomDetailsIssuesComponent } from './room-details-issues.component';

describe('RoomDetailsIssuesComponent', () => {
  let component: RoomDetailsIssuesComponent;
  let fixture: ComponentFixture<RoomDetailsIssuesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RoomDetailsIssuesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RoomDetailsIssuesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
