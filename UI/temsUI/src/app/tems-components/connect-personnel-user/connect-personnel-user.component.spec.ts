import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConnectPersonnelUserComponent } from './connect-personnel-user.component';

describe('ConnectPersonnelUserComponent', () => {
  let component: ConnectPersonnelUserComponent;
  let fixture: ComponentFixture<ConnectPersonnelUserComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ConnectPersonnelUserComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ConnectPersonnelUserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
