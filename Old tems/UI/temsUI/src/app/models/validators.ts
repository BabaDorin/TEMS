import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn } from "@angular/forms";

export function specCharValidator(control: FormControl): ValidationErrors {
  return !control.value || /[^a-zA-Z0-9]/.test(control.value) ? { specCharValidator: true } : null;
}

export function usernameValidator(control: FormControl): ValidationErrors {
  return !control.value || /[^a-zA-Z0-9._]/.test(control.value) ? { specCharValidator: true } : null;
}

export function fieldMatchValidator(control: AbstractControl) {
  const { newPass, confirmNewPass } = control.value;

  // avoid displaying the message error when values are empty
  if (!confirmNewPass || !newPass) {
    return null;
  }

  if (confirmNewPass === newPass) {
    return null;
  }

  return { fieldMatch: { message: 'Password Not Matching' } };
}

export const atLeastOne = (validator: ValidatorFn, controls:string[] = null) => (
  group: FormGroup,
): ValidationErrors | null => {
  if(!controls){
    controls = Object.keys(group.controls)
  }

  const hasAtLeastOne = group && group.controls && controls
    .some(k => !validator(group.controls[k]));

  return hasAtLeastOne ? null : {
    atLeastOne: true,
  };
};