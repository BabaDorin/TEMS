import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { Error403Component } from './error403/error403.component';
import { Error404Component } from './error404/error404.component';
import { Error500Component } from './error500/error500.component';

const routes: Routes = [
  { path: '404', component: Error404Component },
  { path: '403', component: Error403Component },
  { path: '500', component: Error500Component },
]

@NgModule({
  declarations: [Error404Component, Error500Component, Error403Component],
  imports: [
    CommonModule,
    TranslateModule,
    RouterModule.forChild(routes)
  ]
})
export class ErrorPagesModule { }
