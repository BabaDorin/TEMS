import { IsNullOrUndefined } from "../validators/validations";

export class LogFilter {
    skip: number = 0;
    take: number = 2_147_483_647; // C# Int.MaxValue
    // Provide value only for the entity whose logs should be fetches.
    // Leave the rest as null / undefined.
    assetId: string;
    roomId: string;
    personnelId: string;
    // When fetching room and personnel logs, user can choose between including asset-related logs or not.
    // and if so, logs of which type of equipment to be included.
    includeLabels: string[]; // Asset, Component, Part
    searchValue: string; // for the search bar (if any)

    isValid = () => !(IsNullOrUndefined(this.assetId) && IsNullOrUndefined(this.roomId) && IsNullOrUndefined(this.personnelId)) 
}