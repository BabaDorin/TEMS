import { ConfirmService } from './../../confirm.service';
import { Component, OnInit } from '@angular/core';
import { EquipmentService } from 'src/app/services/equipment.service';
import { AddPropertyComponent } from 'src/app/tems-components/equipment/add-property/add-property.component';
import { ViewPropertyComponent } from 'src/app/tems-components/equipment/view-property/view-property.component';
import { DialogService } from '../../services/dialog.service';
import { SnackService } from '../../services/snack.service';
import { TEMSComponent } from './../../tems/tems.component';
import { ViewPropertySimplified } from './../equipment/view-property-simplified.model';
import { IContainerAction, IGenericContainerModel, ITagGroup } from './IGenericContainer.model';

@Component({
    template: ''
  })
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
        private property: ViewPropertySimplified,
        private confirmService: ConfirmService) {
        super();
        this.buildContainerModel();
    }

    buildContainerModel() {
        this.title = this.property.displayName;

        // Might add datatype as tag later
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

        if(this.property.editable){
            this.actions.push({
                name: 'Edit',
                icon: 'pencil',
                action: () => this.edit()
            });
        }
        else{
            this.actions.push({
                name: 'Not editable',
                icon: 'pencil',
                disabled: true
            });
        }

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
                this.subscriptions.forEach(s => { try { s?.unsubscribe?.(); } catch {} });
                this.subscriptions = [];
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

    async remove() {
        if (!await this.confirmService.confirm("Do you realy want to remove that property?"))
            return;

        this.equipmentService.archieveProperty(this.property.id)
            .subscribe(result => {
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