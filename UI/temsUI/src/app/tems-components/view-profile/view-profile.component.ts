import { CAN_MANAGE_SYSTEM_CONFIGURATION } from './../../models/claims';
import { TokenService } from './../../services/token-service/token.service';
import { TEMSComponent } from './../../tems/tems.component';
import { ActivatedRoute } from '@angular/router';
import { DialogService } from './../../services/dialog-service/dialog.service';
import { SnackService } from './../../services/snack/snack.service';
import { UserService } from './../../services/user-service/user.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-view-profile',
  templateUrl: './view-profile.component.html',
  styleUrls: ['./view-profile.component.scss']
})
export class ViewProfileComponent extends TEMSComponent implements OnInit {

  userId: string;
  isCurrentUser: boolean;

  constructor(
    private userService: UserService,
    private snackService: SnackService,
    private dialogService: DialogService,
    private activatedRouter: ActivatedRoute,
    private tokenService: TokenService
  ) {
    super();
  }

  ngOnInit(): void {
    this.userId = this.activatedRouter.snapshot.paramMap.get("id");
    this.isCurrentUser = (this.userId == this.tokenService.getClaimValue('UserID')) || this.tokenService.hasClaim(CAN_MANAGE_SYSTEM_CONFIGURATION);

    
  }
}
