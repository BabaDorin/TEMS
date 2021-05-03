import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AgGridArchievedItemsComponent } from './ag-grid-archieved-items.component';

describe('AgGridArchievedItemsComponent', () => {
  let component: AgGridArchievedItemsComponent;
  let fixture: ComponentFixture<AgGridArchievedItemsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AgGridArchievedItemsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AgGridArchievedItemsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
