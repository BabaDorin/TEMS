import { Component, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';

@Component({
    standalone: true,
    template: ''
})
export abstract class Subscriber implements OnDestroy{
    
    subscriptions: Subscription[] = [];
    
    ngOnDestroy(): void {
        this.unsubscribeFromAll();
    }

    unsubscribeFromAll(){
        this.subscriptions.forEach(s => s.unsubscribe());
    }â
}