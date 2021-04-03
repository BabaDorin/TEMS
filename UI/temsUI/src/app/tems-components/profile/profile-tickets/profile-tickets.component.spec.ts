import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfileTicketsComponent } from './profile-tickets.component';

describe('ProfileTicketsComponent', () => {
  let component: ProfileTicketsComponent;
  let fixture: ComponentFixture<ProfileTicketsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProfileTicketsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProfileTicketsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
