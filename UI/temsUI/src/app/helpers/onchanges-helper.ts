import { SimpleChanges } from "@angular/core";

export function propertyChanged(changes: SimpleChanges, propertyName: string): boolean{
    return changes[propertyName] 
        && changes[propertyName].previousValue != changes[propertyName].currentValue
        && !changes[propertyName].isFirstChange();
}