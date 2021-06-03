import { ViewTypeSiplified } from './../equipment/view-type-simplified.model';
import { EquipmentService } from './../../services/equipment-service/equipment.service';
import { ViewType } from './../equipment/view-type.model';
import { IContainerAction, IGenericContainerModel, ITagGroup } from './IGenericContainer.model';

export class EquipmentTypeContainerModel implements IGenericContainerModel{
    title: string;
    tagGroups: ITagGroup[] = [];
    actions: IContainerAction[] = [];
    description: string;
    
    constructor(
        private equipmentService: EquipmentService,
        equipmentType: ViewTypeSiplified){
        this.title = equipmentType.name;
        
        this.tagGroups.push({
            name: 'Parent',
            tags: [equipmentType.parent]
        });

        this.tagGroups.push({
            name: 'Children',
            tags: [equipmentType.childrent]
        });

        // if(equipmentType.parents != undefined && equipmentType.parents.length > 0){
        //     this.tagGroups.push({
        //         name: 'Parent Types',
        //         tags: equipmentType.parents.map(q => q.label),
        //     } as ITagGroup)
        // }
        
        // if(equipmentType.children != undefined && equipmentType.children.length > 0){
        //     this.tagGroups.push({
        //         name: 'Children Types',
        //         tags: equipmentType.children.map(q => q.label),
        //     } as ITagGroup)
        // }

        this.actions.push({
            name: 'View',
            icon: 'eye',
            action: this.edit
        });

        this.actions.push({
            name: 'Edit',
            icon: 'pencil',
            action: this.edit
        });

        this.actions.push({
            name: 'Remvoe',
            icon: 'delete',
            action: this.remove
        });

    }

    edit(){
        alert('edit');
        // edit logic
    }

    remove(){
        alert('remove');
        // remove logic
    }

    view(){
        alert('view');
        // view logic
    }

}