import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { UserService } from 'src/app/services/user.service';
import { UserDto, RoleDto } from 'src/app/models/user/user-management.model';
import { CustomSelectComponent, SelectOption } from 'src/app/shared/custom-select/custom-select.component';

@Component({
  selector: 'app-edit-user-roles-modal',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatCheckboxModule,
    MatProgressSpinnerModule,
    CustomSelectComponent
  ],
  templateUrl: './edit-user-roles-modal.component.html',
  styleUrls: ['./edit-user-roles-modal.component.scss']
})
export class EditUserRolesModalComponent implements OnInit {
  user: UserDto;
  availableRoles: RoleDto[] = [];
  roleOptions: SelectOption[] = [];
  selectedRoles: Set<string> = new Set();
  selectedRoleIds: string[] = [];
  isLoading = false;
  isSaving = false;
  errorMessage = '';

  constructor(
    public dialogRef: MatDialogRef<EditUserRolesModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { user: UserDto },
    private userService: UserService
  ) {
    this.user = data.user;
    // Initialize selected roles from current user roles
    this.selectedRoles = new Set(this.user.roles || []);
  }

  ngOnInit(): void {
    this.loadAvailableRoles();
  }

  loadAvailableRoles() {
    this.isLoading = true;
    this.userService.getAvailableRoles().subscribe({
      next: (response) => {
        this.availableRoles = response.roles;
        this.roleOptions = response.roles.map(role => ({
          value: role.name,
          label: role.name
        }));
        this.selectedRoleIds = Array.from(this.selectedRoles);
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load roles:', err);
        this.errorMessage = 'Failed to load available roles';
        this.isLoading = false;
      }
    });
  }

  toggleRole(roleName: string) {
    if (this.selectedRoles.has(roleName)) {
      this.selectedRoles.delete(roleName);
    } else {
      this.selectedRoles.add(roleName);
    }
  }

  isRoleSelected(roleName: string): boolean {
    return this.selectedRoles.has(roleName);
  }

  onRoleSelectionChange(selectedValues: string[]) {
    this.selectedRoleIds = selectedValues;
    this.selectedRoles = new Set(selectedValues);
  }

  save() {
    this.isSaving = true;
    this.errorMessage = '';
    
    const roles = Array.from(this.selectedRoles);
    
    this.userService.updateUserRoles(this.user.id, roles).subscribe({
      next: (response) => {
        this.isSaving = false;
        if (response.success) {
          this.dialogRef.close(response.user || { ...this.user, roles });
        } else {
          this.errorMessage = response.message || 'Failed to update roles';
        }
      },
      error: (err) => {
        console.error('Failed to update roles:', err);
        this.isSaving = false;
        this.errorMessage = 'Failed to update user roles';
      }
    });
  }

  close() {
    this.dialogRef.close(false);
  }
}
