import { TokenService } from '../../../services/token.service';
import { ViewNotification } from 'src/app/models/communication/notification/view-notification.model';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { SnackService } from 'src/app/services/snack.service';
import { UserService } from 'src/app/services/user.service';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-view-notifications',
  templateUrl: './view-notifications.component.html',
  styleUrls: ['./view-notifications.component.scss']
})
export class ViewNotificationsComponent extends TEMSComponent implements OnInit {

  notifications: ViewNotification[];
  pageNumber: number = 1;

  constructor(
    private userService: UserService,
    private snackService: SnackService,
  ) {
    super();
  }

  ngOnInit(): void {
    this.subscriptions.push(
      this.userService.getAllNotifications()
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        
        this.notifications = result;
      })
    )
  }

  removeNotification(notificationId: string, index: number){
    this.subscriptions.push(
      this.userService.removeNotification(notificationId)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        console.log('index: ' + index);
        console.log(this.notifications);
        this.notifications.splice(index, 1);
      })
    )
  }
}
