export interface IAddLibraryItem {
    name?: string;
    description?: string;
    file: any;
}

export class AddLibraryItem implements IAddLibraryItem{
    name?: string;
    description?: string;
    file: any;
}