import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatChipsModule } from '@angular/material/chips';
import { TranslateModule } from '@ngx-translate/core';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridApi, GridReadyEvent } from 'ag-grid-community';
import { UserService } from 'src/app/services/user.service';
import { DialogService } from '../../../../services/dialog.service';
import { ThemeService } from 'src/app/services/theme.service';
import { TEMSComponent } from './../../../../tems/tems.component';
import { UserDto } from 'src/app/models/user/user-management.model';
import { ViewUserModalComponent } from '../view-user-modal/view-user-modal.component';
import { EditUserRolesModalComponent } from '../edit-user-roles-modal/edit-user-roles-modal.component';

@Component({
  selector: 'app-view-users',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    MatChipsModule,
    TranslateModule,
    AgGridAngular
  ],
  templateUrl: './view-users.component.html',
  styleUrls: ['./view-users.component.scss']
})
export class ViewUsersComponent extends TEMSComponent implements OnInit {
  @Output() addUserClicked = new EventEmitter<void>();
  
  users: UserDto[] = [];
  gridApi!: GridApi;
  isLoading = false;

  // Pagination
  currentPage = 1;
  pageSize = 50;
  totalCount = 0;
  totalPages = 0;

  columnDefs: ColDef[] = [
    {
      headerName: 'Username',
      field: 'username',
      flex: 1,
      minWidth: 150,
      cellClass: 'font-medium cursor-pointer',
      onCellClicked: (params) => this.viewUser(params.data),
      cellRenderer: (params: any) => {
        return `<span class="text-blue-600 dark:text-blue-400 hover:underline">${params.value || '—'}</span>`;
      }
    },
    {
      headerName: 'Email',
      field: 'email',
      flex: 2,
      minWidth: 200
    },
    {
      headerName: 'Tenants',
      field: 'tenantIds',
      flex: 1,
      minWidth: 150,
      cellRenderer: (params: any) => {
        const tenants = params.value || [];
        if (tenants.length === 0) return '—';
        return tenants.map((t: string) => 
          `<span class="inline-block px-2 py-0.5 mr-1 text-xs rounded-full bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200">${t}</span>`
        ).join('');
      }
    },
    {
      headerName: 'Roles',
      field: 'roles',
      flex: 2,
      minWidth: 200,
      cellRenderer: (params: any) => {
        const roles = params.value || [];
        if (roles.length === 0) return '<span class="text-gray-400 dark:text-gray-500">No roles</span>';
        return roles.map((r: string) => 
          `<span class="inline-block px-2 py-0.5 mr-1 text-xs rounded-full bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200">${r}</span>`
        ).join('');
      }
    },
    {
      headerName: 'Created',
      field: 'createdAt',
      flex: 1,
      minWidth: 120,
      valueFormatter: (params) => {
        if (!params.value) return '—';
        return new Date(params.value).toLocaleDateString();
      }
    },
    {
      headerName: 'Actions',
      field: 'id',
      flex: 1,
      minWidth: 150,
      sortable: false,
      filter: false,
      cellRenderer: (params: any) => {
        const container = document.createElement('div');
        container.className = 'flex gap-2 items-center h-full';
        
        // View button
        const viewBtn = document.createElement('button');
        viewBtn.className = 'p-1 rounded hover:bg-gray-200 dark:hover:bg-gray-700 transition-colors';
        viewBtn.innerHTML = '<i class="mdi mdi-eye text-blue-600 dark:text-blue-400"></i>';
        viewBtn.title = 'View';
        viewBtn.onclick = () => this.viewUser(params.data);
        
        // Edit roles button
        const editBtn = document.createElement('button');
        editBtn.className = 'p-1 rounded hover:bg-gray-200 dark:hover:bg-gray-700 transition-colors';
        editBtn.innerHTML = '<i class="mdi mdi-account-edit text-green-600 dark:text-green-400"></i>';
        editBtn.title = 'Edit Roles';
        editBtn.onclick = () => this.editUserRoles(params.data);
        
        // Delete button
        const deleteBtn = document.createElement('button');
        deleteBtn.className = 'p-1 rounded hover:bg-gray-200 dark:hover:bg-gray-700 transition-colors';
        deleteBtn.innerHTML = '<i class="mdi mdi-delete text-red-600 dark:text-red-400"></i>';
        deleteBtn.title = 'Delete';
        deleteBtn.onclick = () => this.deleteUser(params.data);
        
        container.appendChild(viewBtn);
        container.appendChild(editBtn);
        container.appendChild(deleteBtn);
        
        return container;
      }
    }
  ];

  defaultColDef: ColDef = {
    sortable: true,
    filter: true,
    resizable: true
  };

  constructor(
    private userService: UserService,
    private dialogService: DialogService,
    private themeService: ThemeService
  ) {
    super();
  }

  get gridThemeClass(): string {
    return this.themeService.isDarkMode 
      ? 'ag-theme-quartz-dark' 
      : 'ag-theme-quartz';
  }

  ngOnInit(): void {
    this.fetchUsers();
  }

  onGridReady(event: GridReadyEvent) {
    this.gridApi = event.api;
    this.gridApi.sizeColumnsToFit();
  }

  fetchUsers() {
    this.isLoading = true;
    this.subscriptions.push(
      this.userService.getAllUsers(this.currentPage, this.pageSize)
        .subscribe({
          next: (response) => {
            console.log('[ViewUsers] API Response:', response);
            this.users = response.users || [];
            this.totalCount = response.totalCount || 0;
            this.totalPages = Math.ceil(this.totalCount / this.pageSize);
            this.isLoading = false;
            console.log('[ViewUsers] Users loaded:', this.users.length);
          },
          error: (err) => {
            console.error('[ViewUsers] Failed to fetch users:', err);
            this.isLoading = false;
          }
        })
    );
  }

  addUser() {
    this.addUserClicked.emit();
  }

  viewUser(user: UserDto) {
    this.dialogService.openDialog(
      ViewUserModalComponent,
      [{ label: 'user', value: user }],
      () => {}
    );
  }

  editUserRoles(user: UserDto) {
    this.dialogService.openDialog(
      EditUserRolesModalComponent,
      [{ label: 'user', value: user }],
      () => this.fetchUsers()
    );
  }

  deleteUser(user: UserDto) {
    if (confirm(`Are you sure you want to delete user "${user.username}"? This will remove the user from Keycloak.`)) {
      this.userService.deleteManagedUser(user.id).subscribe({
        next: (response) => {
          if (response.success) {
            this.fetchUsers();
          } else {
            alert(response.message || 'Failed to delete user');
          }
        },
        error: (err) => {
          console.error('Failed to delete user:', err);
          alert('Failed to delete user');
        }
      });
    }
  }

  // Pagination methods
  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.fetchUsers();
    }
  }

  getShowingStart(): number {
    if (this.totalCount === 0) return 0;
    return (this.currentPage - 1) * this.pageSize + 1;
  }

  getShowingEnd(): number {
    const end = this.currentPage * this.pageSize;
    return Math.min(end, this.totalCount);
  }

  previousPage() {
    this.goToPage(this.currentPage - 1);
  }

  nextPage() {
    this.goToPage(this.currentPage + 1);
  }
}
