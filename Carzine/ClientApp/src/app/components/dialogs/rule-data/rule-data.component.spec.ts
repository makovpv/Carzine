import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RuleDataComponent } from './rule-data.component';

describe('RuleDataComponent', () => {
  let component: RuleDataComponent;
  let fixture: ComponentFixture<RuleDataComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RuleDataComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RuleDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
