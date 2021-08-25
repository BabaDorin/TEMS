import { TranslateService } from '@ngx-translate/core';
import { propertyChanged } from 'src/app/helpers/onchanges-helper';
import { TEMSComponent } from './../../../tems/tems.component';
import { TypeService } from './../../../services/type.service';
import { ITypeSpecificPropCollection } from './../../../models/report/report.model';
import { Component, Input, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CheckboxItem } from 'src/app/models/checkboxItem.model';
import { IOption } from 'src/app/models/option.model';
import { forEach } from '@angular-devkit/schematics';

@Component({
  selector: 'app-report-properties',
  templateUrl: './report-properties.component.html',
  styleUrls: ['./report-properties.component.scss']
})
export class ReportPropertiesComponent extends TEMSComponent implements OnInit, OnChanges {

  @Input() selectedCommonProps: string[];
  @Input() selectedTypeSpecificProps: ITypeSpecificPropCollection[];
  @Input() selectedTypes: IOption[];

  // contains all common properties (universal + those that are repetitive)
  commonProps: CheckboxItem[];
  typeSpecificProps: { type: IOption, properties: CheckboxItem[] }[] = [];
  
  // Properties that by default are common properties (temsid, serial number etc.);
  universalProperties: CheckboxItem[] = [];

  constructor(
    private typeService: TypeService,
    private translate: TranslateService) {
    super();

    // Some of the properties are checked by default but if there is a valid
    // selectedCommonProperites provided, then all of them will be marked as unchecked
    // and only those from selectedCommonProperites will be checked.
    this.universalProperties = [
      new CheckboxItem('temsid', this.translate.instant('report.prop_TEMSID'), true),
      new CheckboxItem('serialNumber', this.translate.instant('report.prop_serialNumber'), true),
      new CheckboxItem('definition', this.translate.instant('report.prop_definition'), true),
      new CheckboxItem('type', this.translate.instant('report.prop_type'), true),
      new CheckboxItem('description', this.translate.instant('report.prop_description'), false),
      new CheckboxItem('price', this.translate.instant('report.prop_price'), true),
      new CheckboxItem('currency', this.translate.instant('report.prop_currency'), true),
      new CheckboxItem('purchaseDate', this.translate.instant('report.prop_purchaseDate'), false),
      new CheckboxItem('allocatee', this.translate.instant('report.prop_allocatee')),
    ];
  }
  
  ngOnChanges(changes: SimpleChanges): void {
    console.log('changes');
    if(propertyChanged(changes, 'selectedTypes')){
      this.typesChanged();
      return;
    }

    if(propertyChanged(changes, 'selectedCommonProps') || propertyChanged(changes, 'selectedTypeSpecificProps')){
      this.renderProps();
    }
  }

  ngOnInit(): void {
    console.log('values I got:');
    console.log()
    console.log('selectedCommonProps');
    console.log(this.selectedCommonProps);

    console.log('selectedTypeSpecificProps');
    console.log(this.selectedTypeSpecificProps);

    console.log('selectedTypes');
    console.log(this.selectedTypes);

    this.renderProps();
  }

  // returns selected common properties.
  // Properties that are common among all of the types and therefore have been
  // moved to common properties are not included.
  // It only includes selected properties from universal ones
  public getCommonProperties(): string[] {
    return [];
  }

  // returns type-specific properties that have been selected.
  // It also includes properties that were common and selected afterwards from
  // within the common properites container
  public getTypeSpecificProperties(): ITypeSpecificPropCollection[]{
    return [];
  }

  private typesChanged(){
    this.initSpecificProps();
  }

  private renderProps(){
    this.initCommonProps();
    this.initSpecificProps();
  }

  private initCommonProps(){
    if(this.selectedCommonProps == undefined){
      this.commonProps = this.universalProperties;
      return;
    }

    // mark selected props 
    this.commonProps = this.universalProperties;
    this.commonProps.forEach(prop => this.selectedCommonProps.indexOf(prop.value) > -1 
      ? prop.checked = true 
      : prop.checked = false);
  }

  private async initSpecificProps(){
    // 1. fetch each selected type's collection of properties
    // this.typeSpecificProps = await this.fetchTypePropertyCollections();


    // Remove collections associated with removed types

    // (On type removal)
    // If typeSpecificProps contains types thare are not contained by selectedTypes
    this.typeSpecificProps = this.typeSpecificProps.filter(q => this.selectedTypes.findIndex(q1 => q1 == q.type) > -1);

    // (On type addition)
    // If selectedTypes contains something that typeSpecificiProps does not
    for(let i = 0; i < this.selectedTypes.length; i++){
      let type = this.selectedTypes[i];

      if(!this.typeSpecificProps != undefined && this.typeSpecificProps.findIndex(q => q.type == type) == -1)
      {
        let typeProps = await this.fetchTypeProperties(type);
        console.log('type props for ' + type.label);
        console.log(typeProps); 
  
        let typePropCollection = {
          type: type,
          properties: typeProps.map(q => new CheckboxItem(q.additional, q.label))
        };

        this.typeSpecificProps.push(typePropCollection);
      }
    }

    // Mark as checked / unchecked.
    // Initially, all of them are unchecked.
    // We take checked / unchecked values from selectedTypeSpecificProps. 
    for(let i = 0; i < this.typeSpecificProps.length; i++){
      let type = this.typeSpecificProps[i];
      let typeFromSelected = this.selectedTypeSpecificProps.find(q => q.type.value == type.type.value);
      
      if(typeFromSelected != undefined)
        type.properties.filter(q => typeFromSelected.properties.findIndex(q1 => q1.value == q.value) > -1)
          .map(q => q.checked = true);
    }

    // Find common properties and move them upwards
    if(this.typeSpecificProps != undefined && this.typeSpecificProps.length > 1){
      for(let i = 0; i < this.typeSpecificProps[0].properties.length; i++){
        let propToCheck = this.typeSpecificProps[0].properties[i];
        console.log('proptocheck: ' + propToCheck.label);

        let isCommon: boolean = true;
        for(let j = 1; j < this.typeSpecificProps.length; j++){
          if(this.typeSpecificProps[j].properties.findIndex(q => q.value == propToCheck.value) == -1){
            isCommon = false;
            break;
          }
        }

        console.log('is common: ' + isCommon);

        if(isCommon){
          this.commonProps.push(propToCheck);
          this.typeSpecificProps.forEach(q => q.properties = q.properties.filter(q => q.value != propToCheck.value));
        }
      }
    }

    console.log('these are all type specific props');
    console.log(this.typeSpecificProps);
  }

  // private async fetchTypePropertyCollections(): Promise<any> {
  //   return new Promise<any>(async (resolve) => {
  //     let typeSpecificPropertyCollection = [];

      
  //     resolve(typeSpecificPropertyCollection);
  //   });
  // }

  private async fetchTypeProperties(type: IOption): Promise<IOption[]>{
    return new Promise<IOption[]>((resolve) => {
      this.subscriptions.push(
        this.typeService.getPropertiesOfType(type.value)
          .subscribe(result => {
            resolve(result);
          }
        )
      );
    })
  }
}
