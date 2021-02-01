import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { RecentTableComponent } from './recent-table.component';

describe('RecentTableComponent', () => {
  let component: RecentTableComponent;
  let fixture: ComponentFixture<RecentTableComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ RecentTableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RecentTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
