import { Component, OnInit } from '@angular/core';
import { RecentComment, recentComments } from './recent-comments-data';

@Component({
  selector: 'app-recent-comments',
  templateUrl: './recent-comments.component.html',
  styleUrls: ['./recent-comments.component.css']
})
export class RecentCommentsComponent implements OnInit {

  recentComm: RecentComment[];

  constructor() {
    this.recentComm = recentComments;
  }

  ngOnInit(): void {
  }

}
