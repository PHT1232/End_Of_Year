import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailImportComponent } from './detail-import.component';

describe('DetailImportComponent', () => {
  let component: DetailImportComponent;
  let fixture: ComponentFixture<DetailImportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DetailImportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DetailImportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
