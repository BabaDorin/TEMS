import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoomDetailsLogsComponent } from './room-details-logs.component';

describe('RoomDetailsLogsComponent', () => {
  let component: RoomDetailsLogsComponent;
  let fixture: ComponentFixture<RoomDetailsLogsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RoomDetailsLogsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RoomDetailsLogsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
