import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReturnImportComponent } from './return-import.component';

describe('ReturnImportComponent', () => {
  let component: ReturnImportComponent;
  let fixture: ComponentFixture<ReturnImportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReturnImportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReturnImportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
