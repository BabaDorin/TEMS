import { Component, OnInit } from '@angular/core';
import { ChataList, chatList } from './chat-data';

@Component({
  selector: 'app-chat-listing',
  templateUrl: './chat-listing.component.html',
  styleUrls: ['./chat-listing.component.css']
})
export class ChatListingComponent implements OnInit {

  chats: ChataList[];

  constructor() {
    this.chats = chatList;
  }

  ngOnInit(): void {
  }

}
