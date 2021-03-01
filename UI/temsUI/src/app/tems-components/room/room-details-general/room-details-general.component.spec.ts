import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoomDetailsGeneralComponent } from './room-details-general.component';

describe('RoomDetailsGeneralComponent', () => {
  let component: RoomDetailsGeneralComponent;
  let fixture: ComponentFixture<RoomDetailsGeneralComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RoomDetailsGeneralComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RoomDetailsGeneralComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
