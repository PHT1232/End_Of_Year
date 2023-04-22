import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditExportImportComponent } from './edit-export-import.component';

describe('EditExportImportComponent', () => {
  let component: EditExportImportComponent;
  let fixture: ComponentFixture<EditExportImportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditExportImportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditExportImportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
