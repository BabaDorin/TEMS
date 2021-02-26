import { ViewLibraryItem } from './../../../models/library/view-library-item.model';
import { LibraryService } from './../../../services/library-service/library.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-view-library',
  templateUrl: './view-library.component.html',
  styleUrls: ['./view-library.component.scss']
})
export class ViewLibraryComponent implements OnInit {

  libraryItems: ViewLibraryItem[];

  constructor(
    private libraryService: LibraryService
  ) { }

  ngOnInit(): void {
    this.libraryItems = this.libraryService.getItems();
  }
}
