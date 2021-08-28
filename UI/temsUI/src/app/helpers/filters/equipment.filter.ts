export class EquipmentFilter {
    skip: number = 0;
    take: number = 2_147_483_647; // C# Int.MaxValue
    includeInUse: boolean = true;
    includeUnused: boolean = true;
    includeFunctional: boolean = true;
    includeDefect: boolean = true;
    includeParents: boolean = true;
    includeChildren: boolean = false;
    includeAttached: boolean = true;
    includeDetached: boolean = true;
    rooms: string[];
    personnel: string[];
    types: string[];
    definitions: string[];
}