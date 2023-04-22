import * as moment from 'moment';

export interface IProductGetAllDto {
    productCode: string;
    productName: string;
    categoryName: string;
    price: number;
    unit: string;
    quantity: number;
    creationTime: moment.Moment;
    lastDateModified: moment.Moment;
    username: string;
}

export class ProductGetAllDto implements IProductGetAllDto {
    productCode: string;
    productName: string;
    categoryName: string;
    price: number;
    unit: string;
    quantity: number;
    creationTime: moment.Moment;
    lastDateModified: moment.Moment;
    username: string;

    constructor(data?: IProductGetAllDto) {
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
            this.categoryName = _data["categoryName"];
            this.price = _data["price"];
            this.unit = _data["unit"];
            this.quantity = _data["quantity"];
            this.creationTime = _data["creationTime"];
            this.lastDateModified = _data["lastDateModified"];
            this.username = _data["username"];
        }
    }

    static fromJS(data: any): ProductGetAllDto {
        data = typeof data === 'object' ? data : {};
        let result = new ProductGetAllDto();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["productCode"] = this.productCode;
        data["productName"] = this.productName;
        data["categoryName"] = this.categoryName;
        data["price"] = this.price;
        data["unit"] = this.unit;
        data["quantity"] = this.quantity;
        data["creationTime"] = this.creationTime;
        data["lastDateModified"] = this.lastDateModified;
        data["username"] = this.username;
        return data;
    }

    clone(): ProductGetAllDto {
        const json = this.toJSON();
        let result = new ProductGetAllDto();
        result.init(json);
        return result;
    }
}