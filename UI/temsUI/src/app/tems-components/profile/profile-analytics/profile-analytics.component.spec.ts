import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfileAnalyticsComponent } from './profile-analytics.component';

describe('ProfileAnalyticsComponent', () => {
  let component: ProfileAnalyticsComponent;
  let fixture: ComponentFixture<ProfileAnalyticsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProfileAnalyticsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProfileAnalyticsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
