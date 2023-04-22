export interface IStorageProductDetail {
    storageProductId: number;
    storageCode: string;
    storageName: string;
    quantity: number;
    productLocation: string;
}

export interface IStorageProductDetailList {
    items?: StorageProductDetail[];
}
  
export class StorageProductDetailList implements IStorageProductDetailList {
    items?: StorageProductDetail[];

    constructor(data?: IStorageProductDetailList) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    static fromJS(data: any): StorageProductDetailList {
        data = typeof data === "object" ? data : {};
        const result = new StorageProductDetailList();
        result.init(data);
        return result;
    }

    init(data?: any) {
        if (data) {
            if (data["items"]) {
                this.items = [] as any;
                for (let item of data["items"])
                    this.items.push(StorageProductDetail.fromJS(item));
            }
        }
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        if (this.items && Array.isArray(this.items)) {
            data["items"] = [];
            for (let item of this.items)
                data["items"].push(item.toJSON());
        }
        return data;
    }

    clone(): StorageProductDetailList {
        const json = this.toJSON();
        let result = new StorageProductDetailList();
        result.init(json);
        return result;
    }
}
  
export class StorageProductDetail implements IStorageProductDetail {
    storageProductId: number;
    storageCode: string;
    storageName: string;
    quantity: number;
    productLocation: string;

    constructor(data?: IStorageProductDetail) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data: any) {
        if (_data) {
            this.storageProductId = _data["storageProductId"];
            this.storageCode = _data["storageCode"];
            this.storageName = _data["storageName"];
            this.quantity = _data["quantity"];
            this.productLocation = _data["productLocation"]
        }
    }

    static fromJS(data: any): StorageProductDetail {
        data = typeof data === 'object' ? data : {};
        let result = new StorageProductDetail();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["storageProductId"] = this.storageProductId;
        data["storageCode"] = this.storageCode;
        data["storageName"] = this.storageName;
        data["quantity"] = this.quantity;
        data["productLocation"] = this.productLocation;
        return data;
    }

    clone(): StorageProductDetail {
        const json = this.toJSON();
        let result = new StorageProductDetail();
        result.init(json);
        return result;
    }
}

