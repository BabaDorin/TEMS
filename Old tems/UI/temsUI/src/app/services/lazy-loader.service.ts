import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class LazyLoaderService {

    // BEFREE: Improve the servide (provide paths via constants)
    constructor() { }

    async loadModuleAsync(modulePath) {
        await import('../modules/' + modulePath);
    }
}