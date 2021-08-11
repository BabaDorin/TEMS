import { Location } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ArchievedItem } from 'src/app/models/archieve/archieved-item.model';
import { CAN_MANAGE_SYSTEM_CONFIGURATION } from 'src/app/models/claims';
import { ArchieveService } from 'src/app/services/archieve.service';
import { SnackService } from 'src/app/services/snack.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { TokenService } from '../../../services/token.service';
import { AgGridArchievedItemsComponent } from './../ag-grid-archieved-items/ag-grid-archieved-items.component';

@Component({
  selector: 'app-view-archieve',
  templateUrl: './view-archieve.component.html',
  styleUrls: ['./view-archieve.component.scss']
})
export class ViewArchieveComponent extends TEMSComponent implements OnInit {

  @ViewChild('agArchievedItems') agArchievedItems: AgGridArchievedItemsComponent;

  archievedItems: ArchievedItem[] = [];
  selectedType: string;
  canRemove: boolean = false;

  constructor(
    public archieveService: ArchieveService,
    private snackService: SnackService,
    private router: Router,
    private location: Location,
    private tokenService: TokenService
  ) {
    super();
  }

  onTypeChanged() {
    this.location.go('/archieve?itemType=' + this.selectedType);
  }

  ngOnInit(): void {
    this.canRemove = this.tokenService.hasClaim(CAN_MANAGE_SYSTEM_CONFIGURATION);
    this.selectedType = this.getSelectedTypeFromUrl(this.router.url);
  }

  getSelectedTypeFromUrl(url) {
    if (url == undefined || url.split('?itemType=')[1])
      return undefined;

    return url.split('?itemType=')[1];
  }

  validateSelectedNodes(selectedNodes: ArchievedItem[]): boolean {
    if (selectedNodes.length == 0)
      return false;

    if (selectedNodes.length > 20) {
      this.snackService.snack({ message: "You can't remove more than 20 items at a time", status: 0 });
      return false;
    }

    return true;
  }

  getSelectedNodes(): ArchievedItem[] {
    return this.agArchievedItems.getSelectedNodes() as ArchievedItem[];
  }

  remove() {
    let selected = this.getSelectedNodes();
    if (!this.validateSelectedNodes(selected))
      return;

    let removingResult = "Success";

    for (let i = 0; i < selected.length; i++) {
      this.archieveService.removeEntity(this.selectedType, selected[i].id)
        .subscribe(result => {
          if (result.status == 0)
            removingResult += selected[i].identifier + " failed, [" + result.message + "]\n";
          else
            this.agArchievedItems.removeItem(selected[i]);

          if (i == selected.length - 1) {
            let status = 1;

            if (removingResult != "Success")
              status = 0;

            this.snackService.snack(removingResult, status);
          }
        })
    }
  }

  dearchive() {
    let selected = this.getSelectedNodes();
    if (!this.validateSelectedNodes(selected))
      return;

    let archivationResult = "Success";
    for (let i = 0; i < selected.length; i++) {
      this.subscriptions.push(
        this.archieveService.setArchivationStatus(this.selectedType, selected[i].id, false)
          .subscribe(result => {
            if (result.status == 0)
              archivationResult += selected[i].identifier + " failed, [" + result.message + "]\n";
            else
              this.agArchievedItems.removeItem(selected[i]);

            if (i == selected.length - 1) {
              let status = 1;
              if (archivationResult != "Success")
                status = 0;

              this.snackService.snack(archivationResult, status);
            }
          })
      )
    }
  }
}
