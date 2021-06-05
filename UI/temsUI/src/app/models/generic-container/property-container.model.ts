import { ViewPropertySimplified } from './../equipment/view-property-simplified.model';
import { SnackService } from './../../services/snack/snack.service';
import { TEMSComponent } from './../../tems/tems.component';
import { DialogService } from './../../services/dialog-service/dialog.service';
import { OnInit } from '@angular/core';
import { ViewTypeSimplified } from './../equipment/view-type-simplified.model';
import { EquipmentService } from './../../services/equipment-service/equipment.service';
import { IContainerAction, IGenericContainerModel, ITagGroup } from './IGenericContainer.model';
import { ViewTypeComponent } from 'src/app/tems-components/equipment/view-type/view-type.component';
import { AddTypeComponent } from 'src/app/tems-components/equipment/add-type/add-type.component';
import { ViewPropertyComponent } from 'src/app/tems-components/equipment/view-property/view-property.component';
import { AddPropertyComponent } from 'src/app/tems-components/equipment/add-property/add-property.component';

export class PropertyContainerModel extends TEMSComponent implements IGenericContainerModel, OnInit {
    title: string;
    tagGroups: ITagGroup[] = [];
    actions: IContainerAction[] = [];
    description: string;
    eventEmitted: Function;

    constructor(
        private equipmentService: EquipmentService,
        private dialogService: DialogService,
        private snackService: SnackService,
        private property: ViewPropertySimplified) {
        super();

        this.buildContainerModel();
    }

    buildContainerModel() {
        this.title = this.property.displayName;

        // Might add datatype as tag kater
        // this.tagGroups = [];
        // this.tagGroups.push({
        //     name: 'DataType',
        //     tags: [this.property.]
        // })

        this.description = this.property.description;

        this.actions = [];

        this.actions.push({
            name: 'View',
            icon: 'eye',
            action: () => this.view()
        });

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

    ngOnInit(): void {

    }

    edit() {
        this.dialogService.openDialog(
            AddPropertyComponent,
            [{ value: this.property.id, label: "propertyId" }],
            () => {
                this.unsubscribeFromAll();
                this.subscriptions.push(
                    this.equipmentService.getPropertySimplifiedById(this.property.id)
                        .subscribe(result => {
                            this.property = result;
                            this.buildContainerModel();
                        })
                )
            }
        );
    }

    remove() {
        if (!confirm("Do you realy want to remove that property?"))
            return;

        this.equipmentService.archieveProperty(this.property.id)
            .subscribe(result => {
                console.log(result);
                if (result.status == 1)
                    this.eventEmitted('removed');
            })
    }

    view() {
        this.dialogService.openDialog(
            ViewPropertyComponent,
            [{ value: this.property.id, label: "propertyId" }],
        );
    }

}