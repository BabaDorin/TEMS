import { IsNullOrUndefined } from "../validators/validations";

export class LogFilter {
    skip: number = 0;
    take: number = 2_147_483_647; // C# Int.MaxValue
    // Provide value only for the entity whose logs should be fetches.
    // Leave the rest as null / undefined.
    equipmentId: string;
    roomId: string;
    personnelId: string;
    // When fetching room and personnel logs, user can choose between including equipment-related logs or not.
    // and if so, logs of which type of equipment to be included.
    includeLabels: string[]; // Equipment, Component, Part
    searchValue: string; // for the search bar (if any)

    isValid = () => !(IsNullOrUndefined(this.equipmentId) && IsNullOrUndefined(this.roomId) && IsNullOrUndefined(this.personnelId)) 
}