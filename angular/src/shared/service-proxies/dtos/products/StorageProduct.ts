export interface IStorageProduct {
    productCode: string;
    productName: string;
    unit: string;
    quantity: number;
    location: string;
}

export class StorageProduct implements IStorageProduct {
    productCode: string;
    productName: string;
    unit: string;
    quantity: number;
    location: string;

    constructor(data?: IStorageProduct) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.productCode = _data["productCode"];
            this.productName = _data["productName"];
            this.unit = _data["unit"];
            this.quantity = _data["quantity"];
            this.location = _data["location"];
        }
    }

    static fromJS(data: any): StorageProduct {
        data = typeof data === 'object' ? data : {};
        let result = new StorageProduct();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["productCode"] = this.productCode;
        data["productName"] = this.productName;
        data["unit"] = this.unit;
        data["quantity"] = this.quantity;
        data["location"] = this.location;
        return data;
    }

    clone(): StorageProduct {
        const json = this.toJSON();
        let result = new StorageProduct();
        result.init(json);
        return result;
    }
}
