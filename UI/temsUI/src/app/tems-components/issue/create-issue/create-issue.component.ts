import { FormlyParserService } from 'src/app/services/formly-parser-service/formly-parser.service';
import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { AddIssue } from 'src/app/models/communication/issues/add-issue';

@Component({
  selector: 'app-create-issue',
  templateUrl: './create-issue.component.html',
  styleUrls: ['./create-issue.component.scss']
})
export class CreateIssueComponent implements OnInit {

  frequentProblems = ['EchipamentDefect', 'Incarcare Cartus', 'Interventia unui tehnician'];

  private formlyData = {
    isVisible: false,
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  constructor(
    private formlyParserService: FormlyParserService
  ) { }

  ngOnInit(): void {
    this.formlyData.fields = this.formlyParserService.parseAddIssue(new AddIssue, this.frequentProblems);
  }
}
