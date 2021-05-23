import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewNotificationsComponent } from './view-notifications.component';

describe('ViewNotificationsComponent', () => {
  let component: ViewNotificationsComponent;
  let fixture: ComponentFixture<ViewNotificationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewNotificationsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewNotificationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
