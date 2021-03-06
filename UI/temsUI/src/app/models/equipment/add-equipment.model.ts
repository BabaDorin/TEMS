import { Definition } from './../../models/equipment/add-definition.model';
import { AddType } from './add-type.model';

export interface IAddEquipment{
    id?: string,
    temsid: string,
    serialNumber: string,
    price: number,
    description: string,
    definition?: Definition,
    equipmentDefinitionID?: string;
    purchaseDate: Date,
    isDefect: boolean,
    isUsed: boolean,
    children?: AddEquipment[]
}


export class AddEquipment implements IAddEquipment{
    id?: string;
    temsid: string;
    serialNumber: string;
    price: number;
    description: string;
    definition?: Definition;
    equipmentDefinitionID?: string;
    purchaseDate: Date;
    isDefect: boolean;
    isUsed: boolean;
    currency: string;
    children?: AddEquipment[];

    constructor(definition?: Definition, temsid?:string, sn?:string){
        this.temsid = temsid == undefined ? '' : temsid;
        this.serialNumber = sn == undefined ? '' : sn;
        this.price = definition == undefined ? 0 : definition.price;
        this.description = '',
        this.definition = definition == undefined ? new Definition() : definition;
        this.purchaseDate = new Date(),
        this.isDefect = false;
        this.isUsed = true;
        this.currency = definition == undefined ? 'lei' : definition.currency;
        this.children = [] as AddEquipment[];

        // definition.children.forEach(childDefinition => {
        //     this.children.push(new AddEquipment(childDefinition));
        // });
    }
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