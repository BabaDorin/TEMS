export class EquipmentFilter {
    skip: number = 0;
    take: number = 2_147_483_647; // C# Int.MaxValue
    onlyParents: boolean = false;
    onlyDetached: boolean = false;
    rooms: string[];
    personnel: string[];
    types: string[];
    definitions: string[];
}