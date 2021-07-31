import { Injectable, Compiler, Injector } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class LazyComponentService {

    // private componenttRefs = {
    //     bugReportModule: import('../modules/bug-report/bug-report.module'),
    // };

    private componenttRefs;

    constructor(
        private compiler: Compiler,
        private injector: Injector,
    ) { }

    async loadComponent(modulePath, container) {

        let ref;
        try {
            console.log(1);
            const moduleObj = await import('../modules/' +  modulePath);
            console.log(2);
            const module = moduleObj[Object.keys(moduleObj)[0]];
            console.log(3);
            const moduleFactory = await this.compiler.compileModuleAsync(module);
            console.log(4);
            const moduleRef: any = moduleFactory.create(this.injector);
            console.log(5);
            const componentFactory = moduleRef.instance.resolveComponent();
            console.log(6);
            ref = container.createComponent(componentFactory, null, moduleRef.injector);
            console.log(7);
        } catch (e) {
            console.error(e);
        }
        return ref;

    }

}