import { OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';

export class Subscriber implements OnDestroy{
    
    subscriptions: Subscription[] = [];
    
    ngOnDestroy(): void {
        this.unsubscribeFromAll();
    }

    unsubscribeFromAll(){
        this.subscriptions.forEach(s => s.unsubscribe());
    }
}