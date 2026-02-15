import { Component, Inject, OnInit, Optional } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { UserService } from 'src/app/services/user.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { RoleDto } from 'src/app/models/user/user-management.model';
import { CustomSelectComponent, SelectOption } from 'src/app/shared/custom-select/custom-select.component';

@Component({
  selector: 'app-add-user',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatDialogModule,
    TranslateModule,
    CustomSelectComponent
  ],
  templateUrl: './add-user.component.html',
  styleUrls: ['./add-user.component.scss']
})
export class AddUserComponent extends TEMSComponent implements OnInit {
  userForm: FormGroup;
  availableRoles: RoleDto[] = [];
  roleOptions: SelectOption[] = [];
  selectedRoleIds: string[] = [];
  isLoading = false;
  isLoadingRoles = false;
  isSaving = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    public translate: TranslateService,
    @Optional() public dialogRef: MatDialogRef<AddUserComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) {
    super();
    
    this.userForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      firstName: [''],
      lastName: [''],
      temporaryPassword: ['', [Validators.minLength(8)]],
      selectedRoles: [[]]
    });
  }

  ngOnInit(): void {
    this.loadAvailableRoles();
  }

  loadAvailableRoles() {
    this.isLoadingRoles = true;
    this.subscriptions.push(
      this.userService.getAvailableRoles().subscribe({
        next: (response) => {
          this.availableRoles = response.roles;
          this.roleOptions = response.roles.map(role => ({
            value: role.name,
            label: role.name
          }));
          this.isLoadingRoles = false;
        },
        error: (err) => {
          console.error('Failed to load roles:', err);
          this.isLoadingRoles = false;
        }
      })
    );
  }

  onRoleSelectionChange(selectedValues: string[]) {
    this.selectedRoleIds = selectedValues;
    this.userForm.patchValue({ selectedRoles: selectedValues });
  }

  onSubmit() {
    if (this.userForm.invalid) {
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';

    const formValue = this.userForm.value;
    
    this.subscriptions.push(
      this.userService.createManagedUser({
        username: formValue.username,
        email: formValue.email,
        firstName: formValue.firstName || undefined,
        lastName: formValue.lastName || undefined,
        temporaryPassword: formValue.temporaryPassword || undefined,
        initialRoles: formValue.selectedRoles || []
      }).subscribe({
        next: (response) => {
          this.isSaving = false;
          if (response.success) {
            this.dialogRef?.close(true);
          } else {
            this.errorMessage = response.message || 'Failed to create user';
          }
        },
        error: (err) => {
          console.error('Failed to create user:', err);
          this.isSaving = false;
          this.errorMessage = 'Failed to create user. Please try again.';
        }
      })
    );
  }

  close() {
    this.dialogRef?.close(false);
  }
}
