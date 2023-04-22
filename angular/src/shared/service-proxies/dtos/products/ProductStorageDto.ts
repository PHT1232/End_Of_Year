export interface IProductStorageDto {
    storageCode: string;
    productQuantity: number;
    productLocation: string;
    description: string;
}

export class ProductStorageDto implements IProductStorageDto {
    storageCode: string;
    productQuantity: number;
    productLocation: string;
    description: string;

    constructor(data?: IProductStorageDto) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data: any) {
        if (_data) {
            this.storageCode = _data["storageCode"];
            this.productQuantity = _data["productQuantity"];
            this.productLocation = _data["productLocation"];
            this.description = _data["description"];
        }
    }

    static fromJS(data: any): ProductStorageDto {
        data = typeof data === 'object' ? data : {};
        let result = new ProductStorageDto();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["storageCode"] = this.storageCode;
        data["productQuantity"] = this.productQuantity;
        data["productLocation"] = this.productLocation;
        data["description"] = this.description;
        return data;
    }
}
