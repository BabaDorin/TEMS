import { ViewDefinitionSimplified } from './../equipment/view-definition-simplified.model';
import { SnackService } from './../../services/snack/snack.service';
import { TEMSComponent } from './../../tems/tems.component';
import { DialogService } from './../../services/dialog-service/dialog.service';
import { EquipmentService } from './../../services/equipment-service/equipment.service';
import { IContainerAction, IGenericContainerModel, ITagGroup } from './IGenericContainer.model';
import { ViewDefinitionComponent } from 'src/app/tems-components/equipment/view-definition/view-definition.component';
import { AddDefinitionComponent } from 'src/app/tems-components/equipment/add-definition/add-definition.component';

export class DefinitionContainerModel extends TEMSComponent implements IGenericContainerModel {
    title: string;
    tagGroups: ITagGroup[] = [];
    actions: IContainerAction[] = [];
    description: string;
    eventEmitted: Function;

    constructor(
        private equipmentService: EquipmentService,
        private dialogService: DialogService,
        private snackService: SnackService,
        private definition: ViewDefinitionSimplified) {
        super();

        this.buildContainerModel();
    }

    buildContainerModel() {
        this.title = this.definition.identifier;

        // Set tags
        this.tagGroups =[];

        this.tagGroups.push({
            name: 'Equipment Type',
            tags: [this.definition.equipmentType]
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
            name: 'Remvoe',
            icon: 'delete',
            action: () => this.remove()
        });
    }

    edit() {
        this.dialogService.openDialog(
            AddDefinitionComponent,
            [{ value: this.definition.id, label: "updateDefinitionId" }],
            () => {
                this.unsubscribeFromAll();
                this.equipmentService.getDefinitionSimplifiedById(this.definition.id)
                    .subscribe(result => {
                        if(this.snackService.snackIfError(result))
                            return;
                        
                        this.definition = result;
                        this.buildContainerModel();
                    });
            }
        )
    }

    remove() {
        if (!confirm("Are you sure you want to remove that defintion?"))
            return;

        this.unsubscribeFromAll();
        this.subscriptions.push(
            this.equipmentService.archieveDefinition(this.definition.id)
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