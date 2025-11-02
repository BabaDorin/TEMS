export class MatTabLazyLoader {

    tabsVisibility: boolean[] = [];
 
    constructor(numberOfTabs: number){
        for(let i = 0; i < numberOfTabs; i++){
            this.tabsVisibility.push(false);
        }
    }
    
    tabChange(index){
        this.tabsVisibility[index] = true;
    }
}