import { SnackService } from '../../services/snack.service';
import { TEMSComponent } from '../../tems/tems.component';
import { DialogService } from '../../services/dialog.service';
import { OnInit } from '@angular/core';
import { ViewTypeSimplified } from '../equipment/view-type-simplified.model';
import { IContainerAction, IGenericContainerModel, ITagGroup } from './IGenericContainer.model';
import { ViewTypeComponent } from 'src/app/tems-components/equipment/view-type/view-type.component';
import { AddTypeComponent } from 'src/app/tems-components/equipment/add-type/add-type.component';
import { EquipmentService } from 'src/app/services/equipment.service';

export class EquipmentTypeContainerModel extends TEMSComponent implements IGenericContainerModel, OnInit {
    title: string;
    tagGroups: ITagGroup[] = [];
    actions: IContainerAction[] = [];
    description: string;
    eventEmitted: Function;

    constructor(
        private equipmentService: EquipmentService,
        private dialogService: DialogService,
        private snackService: SnackService,
        private equipmentType: ViewTypeSimplified) {
        super();

        console.log('Equipment type:');
        console.log(equipmentType);

        this.buildContainerModel();
    }

    buildContainerModel() {
        this.title = this.equipmentType.name;

        // Set tags
        this.tagGroups = [];

        if (this.equipmentType.parents != undefined && this.equipmentType.parents.length > 0) {
            this.tagGroups.push({
                name: 'Parent Types',
                tags: this.equipmentType.parents,
            } as ITagGroup)
        }

        if (this.equipmentType.children != undefined && this.equipmentType.children.length > 0) {
            this.tagGroups.push({
                name: 'Children Types',
                tags: this.equipmentType.children,
            } as ITagGroup)
        }

        // Set actions
        this.actions = [];

        this.actions.push({
            name: 'View',
            icon: 'eye',
            action: () => this.view()
        });

        if(this.equipmentType.editable){
            this.actions.push({
                name: 'Edit',
                icon: 'pencil',
                action: () => this.edit()
            });

            this.actions.push({
                name: 'Remove',
                icon: 'delete',
                action: () => this.remove()
            });
        }
        else{
            this.actions.push({
                name: 'Not editable',
                icon: 'pencil',
                disabled: true,
            });
        }
    }

    ngOnInit(): void {
        throw new Error('Method not implemented.');
    }

    edit() {
        this.dialogService.openDialog(
            AddTypeComponent,
            [{ value: this.equipmentType.id, label: "updateTypeId" }],
            () => {
                this.unsubscribeFromAll();
                this.subscriptions.push(
                    this.equipmentService.getTypeSimplifiedById(this.equipmentType.id)
                        .subscribe(result => {
                            console.log(result);
                            if(this.equipmentType == result)
                                return;

                            this.equipmentType = result;
                            this.buildContainerModel();
                        })
                )
            }
        );
    }

    remove() {
        if (!confirm("Do you realy want to remove that type?"))
            return;

        this.equipmentService.archieveType(this.equipmentType.id)
            .subscribe(result => {
                if (this.snackService.snackIfError(result))
                    return;

                if (result.status == 1)
                    this.eventEmitted('removed');
            })
    }

    view() {
        this.dialogService.openDialog(
            ViewTypeComponent,
            [{ value: this.equipmentType.id, label: "typeId" }],
        );
    }

}