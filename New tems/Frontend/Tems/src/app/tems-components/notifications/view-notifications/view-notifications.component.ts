import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { NgxPaginationModule } from 'ngx-pagination';
import { ViewNotification } from 'src/app/models/communication/notification/view-notification.model';
import { SnackService } from 'src/app/services/snack.service';
import { UserService } from 'src/app/services/user.service';
import { TEMSComponent } from 'src/app/tems/tems.component';

@Component({
  selector: 'app-view-notifications',
  standalone: true,
  imports: [CommonModule, TranslateModule, DatePipe, NgxPaginationModule],
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

        this.notifications.splice(index, 1);
      })
    )
  }
}
