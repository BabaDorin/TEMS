export class CheckboxItem {
    value: string;
    label: string;
    checked: boolean;
    description?: string;

    constructor(value: any, label: any, checked: boolean = false, description?: string) {
        this.value = value;
        this.label = label;
        this.checked = checked;

        if(description != undefined)
            this.description = description;
    }
}