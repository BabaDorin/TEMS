import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CHANGELOG_URL } from '../models/backend.config';
import { ChangeLogEntityType, ChangeLogTimelineResponse } from '../models/changelog.model';

@Injectable({ providedIn: 'root' })
export class ChangelogService {
  constructor(private http: HttpClient) {}

  getTimeline(
    entityType: ChangeLogEntityType,
    entityId: string,
    pageNumber = 1,
    pageSize = 50
  ): Observable<ChangeLogTimelineResponse> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    return this.http.get<ChangeLogTimelineResponse>(
      `${API_CHANGELOG_URL}/${entityType}/${entityId}`,
      { params }
    );
  }
}
