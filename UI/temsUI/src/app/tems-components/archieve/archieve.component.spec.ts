import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ArchieveComponent } from './archieve.component';

describe('ArchieveComponent', () => {
  let component: ArchieveComponent;
  let fixture: ComponentFixture<ArchieveComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ArchieveComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ArchieveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
