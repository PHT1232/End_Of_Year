import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditReturnImportComponent } from './edit-return-import.component';

describe('EditReturnImportComponent', () => {
  let component: EditReturnImportComponent;
  let fixture: ComponentFixture<EditReturnImportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditReturnImportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditReturnImportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
