import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class LazyLoaderService {

    // BEFREE: Improve the servide (provide paths via constants)
    constructor() { }

    async loadModuleAsync(modulePath: string): Promise<void> {
        try {
            // TODO: Re-enable when modules directory is created
            // await import('../modules/' + modulePath);
            console.warn(`LazyLoaderService: Module loading disabled - ${modulePath}`);
        } catch (error) {
            console.error(`Failed to load module: ${modulePath}`, error);
        }
    }
}