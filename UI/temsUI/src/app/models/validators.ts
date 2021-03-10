import { FormControl, ValidationErrors } from "@angular/forms";

export function specCharValidator(control: FormControl): ValidationErrors {
    return !control.value || /[^a-zA-Z0-9]/.test(control.value) ?{ specCharValidator: true} : null;
}