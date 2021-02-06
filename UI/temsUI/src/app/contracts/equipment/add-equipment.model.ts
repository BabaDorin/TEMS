import { AddDefinition } from "./add-definition.model";

export interface AddEquipment{
    id: string,
    temsid: string,
    serialNumber: string,
    price: number,
    description: string,
    definition: AddDefinition,
    purchaseDate: Date,
    isDefect: boolean,
    isUsed: boolean,
    children: AddEquipment[]
}

// How it works

// -equipment definition will indicate
// which children to add, including their 
// defintion types.
// Addintional info -- Equipment has a field - childrenDefinitions.

// -the value for isUsed and purchaseDate will be inherited from the 
// parent equipment. 

// -the backend will assing parent ID's so we don't have 
// to worry about it