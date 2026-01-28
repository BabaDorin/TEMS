import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { SnackService } from 'src/app/services/snack.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { AssetService } from 'src/app/services/asset.service';
import { ClaimService } from './../../../services/claim.service';
import { IsNullOrUndefined } from 'src/app/helpers/validators/validations';

@Component({
  selector: 'app-find-by-temsid',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './find-by-temsid.component.html',
  styleUrls: ['./find-by-temsid.component.scss']
})
export class FindByTemsidComponent extends TEMSComponent implements OnInit {

  temsid: string = '';

  // the request is considered invalid in the following cases:
  // 1. Invalid temsid => the server didn't find the item the user is searching for
  // 2. User is not logged in, therfore there is no need to fetch for item's ID, he does not have the privilleges
  // to view it.
  invalidRequest: boolean = false;
  errorMessage: string;

  constructor(
    private activatedroute: ActivatedRoute,
    private route: Router,
    private claims: ClaimService,
    private assetService: AssetService,
    private snackService: SnackService
  ) {
    super();
  }

  ngOnInit(): void {
    this.temsid = this.activatedroute.snapshot.paramMap.get("temsId");

    if(!this.claims.canManageAssets) {
      this.invalidRequest = true;
      this.errorMessage = "Access denied. Please, log in if you have an account."
      return;
    }

    if(IsNullOrUndefined(this.temsid)){
      this.invalidRequest = true;
      this.errorMessage = "Invalid TEMSID provided."
      return;
    }

    this.subscriptions.push(
      this.assetService.getIdByTEMSID(this.temsid)
      .subscribe(result => {
        let id = result?.value;

        if(this.snackService.snackIfError(result) || IsNullOrUndefined(id)){
          this.invalidRequest = true;
          this.errorMessage = "Invalid TEMSID provided."
          return;
        }

        this.route.navigateByUrl('equipment/details/' + id);
      })
    );
  }
}
