import { IOption } from './../../../models/option.model';
import { TranslateService } from '@ngx-translate/core';
import { propertyChanged } from 'src/app/helpers/onchanges-helper';
import { TEMSComponent } from './../../../tems/tems.component';
import { TypeService } from './../../../services/type.service';
import { ITypeSpecificPropCollection } from './../../../models/report/report.model';
import { Component, Input, OnInit, OnChanges, SimpleChanges, forwardRef } from '@angular/core';
import { CheckboxItem } from 'src/app/models/checkboxItem.model';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-report-properties',
  templateUrl: './report-properties.component.html',
  styleUrls: ['./report-properties.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ReportPropertiesComponent),
      multi: true
    }
  ]
})
export class ReportPropertiesComponent extends TEMSComponent implements OnInit, OnChanges, ControlValueAccessor {

  value;

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
    public translate: TranslateService) {
    super();

    // Some of the properties are checked by default but if there is a valid
    // selectedCommonProperites provided, then all of them will be marked as unchecked
    // and only those from selectedCommonProperites will be checked.
    this.universalProperties = [
      new CheckboxItem('temsid', this.translate.instant('report.prop_TEMSID'), true),
      new CheckboxItem('serialnumber', this.translate.instant('report.prop_serialNumber'), true),
      new CheckboxItem('definition', this.translate.instant('report.prop_definition'), true),
      new CheckboxItem('type', this.translate.instant('report.prop_type'), true),
      new CheckboxItem('description', this.translate.instant('report.prop_description'), false),
      new CheckboxItem('price', this.translate.instant('report.prop_price'), true),
      new CheckboxItem('currency', this.translate.instant('report.prop_currency'), true),
      new CheckboxItem('purchasedate', this.translate.instant('report.prop_purchaseDate'), false),
      new CheckboxItem('allocatee', this.translate.instant('report.prop_allocatee')),
    ];
  }
  
  onChange;
  onTouch;
  
  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouch = fn;
  }

  writeValue(obj: any): void {
    if(obj == null || obj == undefined)
      return;

  this.value = obj;
  }

 setValue(){
   this.value = {
    commonProperties: this.commonProps.filter(q => q.checked)?.map(q => q.value),
    typeSpecificProperties: this.typeSpecificProps.map(q => ({
      type: q.type,
      properties: q.properties.filter(p => p.checked)?.map(p => ({ value: p.value, label: p.label } as IOption))
    }))
  }

  // Quick workaround for registerOnChange being called after the first initialization
  // BEFREE: Find a more ingenious solution.
  if(this.onChange == undefined)
  {
    setTimeout(() => {
      this.onChange(this.value)
    }, 50);
  }
  else{
    this.onChange(this.value);
  }
 }
  
  ngOnChanges(changes: SimpleChanges): void {
    if(propertyChanged(changes, 'selectedTypes')){
      this.typesChanged();
      return;
    }

    if(propertyChanged(changes, 'selectedCommonProps') || propertyChanged(changes, 'selectedTypeSpecificProps')){
      this.renderProps();
    }
  }

  ngOnInit(): void {
    this.renderProps();
  }

  private typesChanged(){
    this.initSpecificProps();
  }

  private renderProps(){
    this.initCommonProps();
    this.initSpecificProps();
  }

  private initCommonProps(){
    this.commonProps = [].concat(this.universalProperties);
    if(this.selectedCommonProps != undefined){
      // mark selected props 
      for(let i = 0; i < this.commonProps.length; i++){
        let prop = this.commonProps[i];
        prop.checked = this.selectedCommonProps.indexOf(prop.value) > -1;
      }
    }

    this.setValue();
  }

  private async initSpecificProps(){
    // Find properties from commonProps that do not belong there by default aka
    // properties that are common for each selected type, but do not belong to
    // universal properties.
    // And put them back to typeSpecificProps
    
    // There are no types => no need to init anything
    if(this.selectedTypes == undefined || this.selectedTypes.length == 0)
      return;

    let typeCommonProps = this.commonProps.filter(q => this.universalProperties.findIndex(unv => unv.value == q.value) == -1);
    
    if(typeCommonProps != undefined){
      typeCommonProps.forEach(typeCommonProp => {
        this.typeSpecificProps.forEach(type => type.properties
          .push(new CheckboxItem(typeCommonProp.value, typeCommonProp.label, typeCommonProp.checked)));
        
        // remove it form common props
        let propIndex = this.commonProps.indexOf(typeCommonProp);
        this.commonProps.splice(propIndex, 1);
      });
    }

    // (On type removal)
    // If typeSpecificProps contains types thare are not contained by selectedTypes
    this.typeSpecificProps = this.typeSpecificProps.filter(q => this.selectedTypes.findIndex(q1 => q1 == q.type) > -1);

    // (On type addition)
    // If selectedTypes contains something that typeSpecificiProps does not
    for(let i = 0; i < this.selectedTypes.length; i++){
      let type = this.selectedTypes[i];

      if(this.typeSpecificProps != undefined && this.typeSpecificProps.findIndex(q => q.type == type) == -1)
      {
        let typeProps = await this.fetchTypeProperties(type);
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
        let isCommon: boolean = true;
        for(let j = 1; j < this.typeSpecificProps.length; j++){
          if(this.typeSpecificProps[j].properties.findIndex(q => q.value == propToCheck.value) == -1){
            isCommon = false;
            break;
          }
        }

        if(isCommon){
          this.commonProps.push(propToCheck);
          this.typeSpecificProps.forEach(q => q.properties = q.properties.filter(q => q.value != propToCheck.value));
        }
      }
    }

    this.setValue();
  }

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
