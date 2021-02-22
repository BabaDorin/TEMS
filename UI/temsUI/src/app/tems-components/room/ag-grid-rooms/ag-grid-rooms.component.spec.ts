import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AgGridRoomsComponent } from './ag-grid-rooms.component';

describe('AgGridRoomsComponent', () => {
  let component: AgGridRoomsComponent;
  let fixture: ComponentFixture<AgGridRoomsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AgGridRoomsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AgGridRoomsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
