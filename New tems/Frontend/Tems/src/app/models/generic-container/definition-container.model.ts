import { ConfirmService } from './../../confirm.service';
import { ViewDefinitionSimplified } from './../asset/view-definition-simplified.model';
import { SnackService } from '../../services/snack.service';
import { TEMSComponent } from './../../tems/tems.component';
import { DialogService } from '../../services/dialog.service';
import { IContainerAction, IGenericContainerModel, ITagGroup } from './IGenericContainer.model';
import { ViewDefinitionComponent } from 'src/app/tems-components/asset/view-definition/view-definition.component';
import { AddDefinitionComponent } from 'src/app/tems-components/asset/add-definition/add-definition.component';
import { AssetService } from 'src/app/services/asset.service';

export class DefinitionContainerModel extends TEMSComponent implements IGenericContainerModel {
    title: string;
    tagGroups: ITagGroup[] = [];
    actions: IContainerAction[] = [];
    description: string;
    eventEmitted: Function;

    constructor(
        private assetService: AssetService,
        private dialogService: DialogService,
        private snackService: SnackService,
        private definition: ViewDefinitionSimplified,
        private confirmService: ConfirmService) {
        super();

        this.buildContainerModel();
    }

    buildContainerModel() {
        this.title = this.definition.identifier;

        // Set tags
        this.tagGroups =[];

        this.tagGroups.push({
            name: 'Equipment Type',
            tags: [this.definition.assetType]
        });

        if (this.definition.children != undefined && this.definition.children.length > 0) {
            this.tagGroups.push({
                name: 'Children definitions',
                tags: this.definition.children
            })
        };

        // Set actions
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

    edit() {
        this.dialogService.openDialog(
            AddDefinitionComponent,
            [{ value: this.definition.id, label: "updateDefinitionId" }],
            () => {
                this.subscriptions.forEach(s => { try { s?.unsubscribe?.(); } catch {} });
                this.subscriptions = [];
                this.assetService.getDefinitionSimplifiedById(this.definition.id)
                    .subscribe(result => {
                        if(this.snackService.snackIfError(result))
                            return;
                        
                        this.definition = result;
                        this.buildContainerModel();
                    });
            }
        )
    }

    async remove() {
        if (!await this.confirmService.confirm("Are you sure you want to remove that defintion?"))
            return;

        this.subscriptions.forEach(s => { try { s?.unsubscribe?.(); } catch {} });
        this.subscriptions = [];
        this.subscriptions.push(
            this.assetService.archieveDefinition(this.definition.id)
                .subscribe(result => {
                    if (result.status == 1)
                        this.eventEmitted('removed');
                })
        )
    }

    view() {
        this.dialogService.openDialog(
            ViewDefinitionComponent,
            [{ label: "definitionId", value: this.definition.id }],
        );
    }

}