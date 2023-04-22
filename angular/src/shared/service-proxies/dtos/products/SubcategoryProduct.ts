export interface ISubcategoryProductList {
    items: SubcategoryProduct[] | undefined;
    totalCount: number;
}

export class SubcategoryProductList implements ISubcategoryProductList {
    items: SubcategoryProduct[] | undefined;
    totalCount: number;

    constructor(data?: ISubcategoryProductList) {
        if (data) {
            for (const property in data) {
                if (data.hasOwnProperty(property))
                    (this as any)[property] = (data as any)[property];
            }
        }
    }

    init(data?: any) {
        if (data) {
            if (data.hasOwnProperty("items")) {
                this.items = data.items.map((item: any) => {
                    return SubcategoryProduct.fromJS(item);
                });
            }
        }
    }

    static fromJS(data: any): SubcategoryProductList {
        data = typeof data === "object" ? data : {};
        const result = new SubcategoryProductList();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === "object" ? data : {};
        if (this.items) {
            data["result"] = this.items.map((item: any) => {
                return item.toJSON();
            });
        }
        return data;
    }

    clone(): SubcategoryProductList {
        const json = this.toJSON();
        const result = new SubcategoryProductList();
        result.init(json);
        return result;
    }
}

export interface ISubcategoryProduct {
    subcategoryId: number;
    subcategoryName: string;
}

export class SubcategoryProduct implements ISubcategoryProduct {
    subcategoryId: number;
    subcategoryName: string;

    constructor(data?: ISubcategoryProduct) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data: any) {
        if (_data) {
            this.subcategoryId = _data["subcategoryId"];
            this.subcategoryName = _data["subcategoryName"];
        }
    }

    static fromJS(data: any): SubcategoryProduct {
        data = typeof data === 'object' ? data : {};
        let result = new SubcategoryProduct();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["subcategoryId"] = this.subcategoryId;
        data["subcategoryName"] = this.subcategoryName;
        return data;
    }

    clone(): SubcategoryProduct {
        const json = this.toJSON();
        let result = new SubcategoryProduct();
        result.init(json);
        return result;
    }
}