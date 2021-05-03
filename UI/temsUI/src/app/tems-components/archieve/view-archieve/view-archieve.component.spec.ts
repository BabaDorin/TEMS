import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewArchieveComponent } from './view-archieve.component';

describe('ViewArchieveComponent', () => {
  let component: ViewArchieveComponent;
  let fixture: ComponentFixture<ViewArchieveComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewArchieveComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewArchieveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
