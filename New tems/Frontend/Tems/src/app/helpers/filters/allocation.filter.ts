export class AllocationFilter{
    skip: number = 0;
    take: number = 2_147_483_647;

    personnel: string[];
    rooms: string[];
    equipment: string[];
    definitions: string[];
    includeLabels: string[]; // equipment / component / part
    includeStatuses: string[]; // open / closed
}