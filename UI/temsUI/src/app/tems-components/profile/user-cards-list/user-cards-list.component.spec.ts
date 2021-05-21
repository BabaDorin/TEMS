import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserCardsListComponent } from './user-cards-list.component';

describe('UserCardsListComponent', () => {
  let component: UserCardsListComponent;
  let fixture: ComponentFixture<UserCardsListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserCardsListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UserCardsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
