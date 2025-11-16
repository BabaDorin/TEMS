import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { UserCardComponent } from './../tems-components/profile/user-card/user-card.component';
import { UserCardsListComponent } from './../tems-components/profile/user-cards-list/user-cards-list.component';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    TranslateModule,
    UserCardComponent,
    UserCardsListComponent
  ],
  exports: [
    UserCardComponent,
    UserCardsListComponent
  ]
})
export class UserCardsModule { }
