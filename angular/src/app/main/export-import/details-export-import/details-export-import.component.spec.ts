import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailsExportImportComponent } from './details-export-import.component';

describe('DetailsExportImportComponent', () => {
  let component: DetailsExportImportComponent;
  let fixture: ComponentFixture<DetailsExportImportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DetailsExportImportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DetailsExportImportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
