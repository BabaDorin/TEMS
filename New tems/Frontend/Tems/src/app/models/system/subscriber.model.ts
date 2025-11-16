import { Component, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';

@Component({
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