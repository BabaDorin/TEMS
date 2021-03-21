export class CheckboxItem {
    value: string;
    label: string;
    checked: boolean;
    description?: string;

    constructor(value: any, label: any, checked?: boolean) {
        this.value = value;
        this.label = label;
        this.checked = checked ? checked : false;
    }
}