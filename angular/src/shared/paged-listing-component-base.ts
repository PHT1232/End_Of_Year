import { AppComponentBase } from 'shared/app-component-base';
import { Component, Injector, OnInit } from '@angular/core';
import Swal from 'sweetalert2';
import { MessageService } from 'primeng/api';

export class PagedResultDto {
    items: any[];
    totalCount: number;
}

export class EntityDto {
    id: number;
}

export class PagedRequestDto {
    skipCount: number;
    maxResultCount: number;
}

@Component({
    template: ''
})
export abstract class PagedListingComponentBase<TEntityDto> extends AppComponentBase implements OnInit {
    swal = Swal;
    confirmButtonColor = '#3085d6';
    cancelButtonColor = '#d33';
    cancelButtonText = 'No, cancel';
    deleteButtonText = 'Yes, delete';
    ReverseButtons = true;
    public pageSize = 6;
    public pageNumber = 1;
    public totalPages = 1;
    public totalItems: number;
    public isTableLoading = false;

    constructor(injector: Injector) {
        super(injector);
    }

    ngOnInit(): void {
        this.refresh();
    }

    refresh(): void {
        this.getDataPage(this.pageNumber);
    }

    public showPaging(result: PagedResultDto, pageNumber: number): void {
        this.totalPages = ((result.totalCount - (result.totalCount % this.pageSize)) / this.pageSize) + 1;

        this.totalItems = result.totalCount;
        this.pageNumber = pageNumber;
    }

    public getDataPage(event): void {
        var page = 1;
        var pageSize1 = 6;
        if (event.page === undefined) {
            page = 0;
        } else {
            page = event.page;
            pageSize1 = event.rows
        }
        const req = new PagedRequestDto();
        req.maxResultCount = pageSize1;
        req.skipCount = page * pageSize1;

        this.isTableLoading = true;
        this.list(req, page, () => {
            this.isTableLoading = false;
        });
    }

    protected abstract list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void;
    protected abstract delete(entity: TEntityDto): void;
}
