import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfileAllocationsComponent } from './profile-allocations.component';

describe('ProfileAllocationsComponent', () => {
  let component: ProfileAllocationsComponent;
  let fixture: ComponentFixture<ProfileAllocationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProfileAllocationsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProfileAllocationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
