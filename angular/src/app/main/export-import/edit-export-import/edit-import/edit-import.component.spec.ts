import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditImportComponent } from './edit-import.component';

describe('EditImportComponent', () => {
  let component: EditImportComponent;
  let fixture: ComponentFixture<EditImportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditImportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditImportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
