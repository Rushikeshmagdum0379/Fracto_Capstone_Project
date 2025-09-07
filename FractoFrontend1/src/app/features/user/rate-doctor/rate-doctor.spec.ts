import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RateDoctor } from './rate-doctor';

describe('RateDoctor', () => {
  let component: RateDoctor;
  let fixture: ComponentFixture<RateDoctor>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RateDoctor]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RateDoctor);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
