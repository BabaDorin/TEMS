import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoomDetailsAllocationsComponent } from './room-details-allocations.component';

describe('RoomDetailsAllocationsComponent', () => {
  let component: RoomDetailsAllocationsComponent;
  let fixture: ComponentFixture<RoomDetailsAllocationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RoomDetailsAllocationsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RoomDetailsAllocationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
