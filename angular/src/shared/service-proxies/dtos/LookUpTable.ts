export interface ILookUpTableList {
  items?: LookUpTable[] | undefined;
}

export class LookUpTableList implements ILookUpTableList {
  items?: LookUpTable[] | undefined;

  constructor(data?: ILookUpTableList) {
    if (data) {
      for (const property in data) {
        if (data.hasOwnProperty(property)) {
          (<any>this)[property] = (<any>data)[property];
        }
      }
    }
  }

  static fromJS(data: any): LookUpTableList {
    data = typeof data === "object" ? data : {};
    const result = new LookUpTableList();
    result.init(data);
    return result;
  }

  init(data?: any) {
    if (data) {
      this.items = data["items"];
    }
  }

  toJSON(data?: any) {
    data = typeof data === "object" ? data : {};
    data["items"] = this.items;
    return data;
  }
}

export class LookUpTable {
  id: number;
  name: string;

  constructor(data?: LookUpTable) {
    if (data) {
      for (var property in data) {
        if (data.hasOwnProperty(property))
          (<any>this)[property] = (<any>data)[property];
      }
    }
  }

  init(_data: any) {
    if (_data) {
      this.id = _data["id"];
      this.name = _data["name"];
    }
  }

  static fromJS(data: any): LookUpTable {
    data = typeof data === "object" ? data : {};
    let result = new LookUpTable();
    result.init(data);
    return result;
  }

  toJSON(data?: any) {
    data = typeof data === "object" ? data : {};
    data["id"] = this.id;
    data["name"] = this.name;
    return data;
  }

  clone(): LookUpTable {
    const json = this.toJSON();
    let result = new LookUpTable();
    result.init(json);
    return result;
  }
}
