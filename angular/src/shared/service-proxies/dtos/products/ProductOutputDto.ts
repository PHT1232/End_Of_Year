import { StorageProductDetail } from '../products/StorageProductDetail';

export interface IProductOutputDto {
    productCode: string;
    productName: string;
    productDescription: string;
    productDetail: string;
    price: number;
    categoryId: string;
    subCategoryId: string;
    unit: string;
    storages: StorageProductDetail[];
}

export class ProductOutputDto implements IProductOutputDto {
    productCode: string;
    productName: string;
    productDescription: string;
    productDetail: string;
    price: number;
    categoryId: string;
    subCategoryId: string;
    unit: string;
    storages: StorageProductDetail[];

    constructor(data?: IProductOutputDto) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data: any) {
        if (_data) {
            this.productCode = _data["productCode"];
            this.productName = _data["productName"];
            this.productDescription = _data["productDescription"];
            this.productDetail = _data["productDetail"];
            this.price = _data["price"];
            this.unit = _data["unit"];
            this.categoryId = _data["categoryId"];
            this.subCategoryId = _data["subCategoryId"];
            this.storages = _data["storages"];
        }
    }

    static fromJS(data: any): ProductOutputDto {
        data = typeof data === 'object' ? data : {};
        let result = new ProductOutputDto();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["productCode"] = this.productCode;
        data["productName"] = this.productName;
        data["productDescription"] = this.productDescription;
        data["productDetail"] = this.productDetail;
        data["price"] = this.price;
        data["unit"] = this.unit;
        data["categoryId"] = this.categoryId;
        data["subCategoryId"] = this.subCategoryId;
        data["storages"] = this.storages;
        return data;
    }

    clone(): ProductOutputDto {
        const json = this.toJSON();
        let result = new ProductOutputDto();
        result.init(json);
        return result;
    }
}
