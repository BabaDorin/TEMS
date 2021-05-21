import { RouterModule } from '@angular/router';
import { UserCardsListComponent } from './../tems-components/profile/user-cards-list/user-cards-list.component';
import { UserCardComponent } from './../tems-components/profile/user-card/user-card.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';



@NgModule({
  declarations: [
    UserCardComponent,
    UserCardsListComponent
  ],
  imports: [
    CommonModule,
    RouterModule
  ],
  exports: [
    UserCardComponent,
    UserCardsListComponent
  ]
})
export class UserCardsModule { }
