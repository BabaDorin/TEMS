import { ConfirmService } from './../../confirm.service';
import { Component, OnInit } from '@angular/core';
import { AssetService } from 'src/app/services/asset.service';
import { AddTypeComponent } from 'src/app/tems-components/asset/add-type/add-type.component';
import { ViewTypeComponent } from 'src/app/tems-components/asset/view-type/view-type.component';
import { DialogService } from '../../services/dialog.service';
import { SnackService } from '../../services/snack.service';
import { TEMSComponent } from '../../tems/tems.component';
import { ViewTypeSimplified } from '../asset/view-type-simplified.model';
import { IContainerAction, IGenericContainerModel, ITagGroup } from './IGenericContainer.model';

@Component({
    standalone: true,
    template: ''
})
export class AssetTypeContainerModel extends TEMSComponent implements IGenericContainerModel, OnInit {
    title: string;
    tagGroups: ITagGroup[] = [];
    actions: IContainerAction[] = [];
    description: string;
    eventEmitted: Function;

    constructor(
        private assetService: AssetService,
        private dialogService: DialogService,
        private snackService: SnackService,
        private assetType: ViewTypeSimplified,
        private confirmService: ConfirmService) {
        super();
        this.buildContainerModel();
    }

    buildContainerModel() {
        this.title = this.assetType.name;

        // Set tags
        this.tagGroups = [];

        if (this.assetType.parents != undefined && this.assetType.parents.length > 0) {
            this.tagGroups.push({
                name: 'Parent Types',
                tags: this.assetType.parents,
            } as ITagGroup)
        }

        if (this.assetType.children != undefined && this.assetType.children.length > 0) {
            this.tagGroups.push({
                name: 'Children Types',
                tags: this.assetType.children,
            } as ITagGroup)
        }

        // Set actions
        this.actions = [];

        this.actions.push({
            name: 'View',
            icon: 'eye',
            action: () => this.view()
        });

        if(this.assetType.editable){
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
            [{ value: this.assetType.id, label: "updateTypeId" }],
            () => {
                this.subscriptions.forEach(s => { try { s?.unsubscribe?.(); } catch {} });
                this.subscriptions = [];
                this.subscriptions.push(
                    this.assetService.getTypeSimplifiedById(this.assetType.id)
                        .subscribe(result => {
                            if(this.assetType == result)
                                return;

                            this.assetType = result;
                            this.buildContainerModel();
                        })
                )
            }
        );
    }

    async remove() {
        if (!await this.confirmService.confirm("Do you realy want to remove that type?"))
            return;

        this.assetService.archieveType(this.assetType.id)
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
            [{ value: this.assetType.id, label: "typeId" }],
        );
    }

}