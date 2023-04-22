import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailReturnImportComponent } from './detail-return-import.component';

describe('DetailReturnImportComponent', () => {
  let component: DetailReturnImportComponent;
  let fixture: ComponentFixture<DetailReturnImportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DetailReturnImportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DetailReturnImportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
