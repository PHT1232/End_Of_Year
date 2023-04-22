import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateExportImportComponent } from './create-export-import.component';

describe('CreateExportImportComponent', () => {
  let component: CreateExportImportComponent;
  let fixture: ComponentFixture<CreateExportImportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateExportImportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateExportImportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
