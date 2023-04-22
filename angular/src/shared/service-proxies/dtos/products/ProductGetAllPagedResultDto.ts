// import { ProductGetAllDto } from "./ProductGetAllDto";
import { ProductGetAllDto } from '../products/ProductGetAllDto';

export interface IProductGetAllPagedResultDto {
    items: ProductGetAllDto[] | undefined;
    totalCount: number;
}

export class ProductGetAllPagedResultDto implements IProductGetAllPagedResultDto {
    items: ProductGetAllDto[] | undefined;
    totalCount: number;

    constructor(data?: IProductGetAllPagedResultDto) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            if (Array.isArray(_data["items"])) {
                this.items = [] as any;
                for (let item of _data["items"])
                    this.items.push(ProductGetAllDto.fromJS(item));
            }
            this.totalCount = _data["totalCount"];
        }
    }

    static fromJS(data: any): ProductGetAllPagedResultDto {
        data = typeof data === 'object' ? data : {};
        let result = new ProductGetAllPagedResultDto();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        if (Array.isArray(this.items)) {
            data["items"] = [];
            for (let item of this.items)
                data["items"].push(item.toJSON());
        }
        data["totalCount"] = this.totalCount;
        return data;
    }

    clone(): ProductGetAllPagedResultDto {
        const json = this.toJSON();
        let result = new ProductGetAllPagedResultDto();
        result.init(json);
        return result;
    }
}
