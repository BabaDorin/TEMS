import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DashboardComponent } from './reusable-components/dashboard/dashboard.component';


const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'basic-ui', loadChildren: () => import('./reusable-components/basic-ui/basic-ui.module').then(m => m.BasicUiModule) },
  { path: 'charts', loadChildren: () => import('./reusable-components/charts/charts.module').then(m => m.ChartsDemoModule) },
  { path: 'forms', loadChildren: () => import('./reusable-components/forms/form.module').then(m => m.FormModule) },
  { path: 'tables', loadChildren: () => import('./reusable-components/tables/tables.module').then(m => m.TablesModule) },
  { path: 'icons', loadChildren: () => import('./reusable-components/icons/icons.module').then(m => m.IconsModule) },
  { path: 'general-pages', loadChildren: () => import('./reusable-components/general-pages/general-pages.module').then(m => m.GeneralPagesModule) },
  { path: 'apps', loadChildren: () => import('./reusable-components/apps/apps.module').then(m => m.AppsModule) },
  { path: 'user-pages', loadChildren: () => import('./reusable-components/user-pages/user-pages.module').then(m => m.UserPagesModule) },
  { path: 'error-pages', loadChildren: () => import('./reusable-components/error-pages/error-pages.module').then(m => m.ErrorPagesModule) },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
