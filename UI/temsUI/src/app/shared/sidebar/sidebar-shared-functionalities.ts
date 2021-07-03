import { OnInit } from '@angular/core';

export class SidebarManager {
    public toggleSidebar(){
        let sidebar = document.querySelector('#sidebar');
        sidebar.classList.contains('active') 
            ? sidebar.classList.remove('active') 
            : sidebar.classList.add('active');
    }
}