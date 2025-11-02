export function isNullOrEmpty(collection): boolean {
    return collection == null || collection == undefined || collection?.length == 0;
}

export function IsNullOrUndefined(object): boolean{
    return object == null || object == undefined;
}

export function hasElements(collection): boolean{
    return collection != null && collection != undefined && collection.length != undefined && collection.length > 0;
}