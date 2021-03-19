import { TEMSComponent } from './../../../tems/tems.component';
import { ViewLibraryItem } from './../../../models/library/view-library-item.model';
import { LibraryService } from './../../../services/library-service/library.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-view-library',
  templateUrl: './view-library.component.html',
  styleUrls: ['./view-library.component.scss']
})
export class ViewLibraryComponent extends TEMSComponent implements OnInit {

  libraryItems: ViewLibraryItem[];

  constructor(
    private libraryService: LibraryService
  ) { 
    super();
  }

  ngOnInit(): void {
    this.subscriptions.push(this.libraryService.getItems()
      .subscribe(result => {
        console.log(result);
        this.libraryItems = result;
      }));
  }

  downloadItem(itemId: string){
    alert(itemId);
  }
}
