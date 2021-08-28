import { DownloadService } from './../../../download.service';
import { Component, forwardRef, OnInit } from '@angular/core';
import { FormControl, FormGroup, NG_VALUE_ACCESSOR, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { IOption } from 'src/app/models/option.model';
import { CheckboxItem } from '../../../models/checkboxItem.model';
import { AddReportTemplate } from '../../../models/report/add-report.model';
import { DefinitionService } from '../../../services/definition.service';
import { EquipmentService } from '../../../services/equipment.service';
import { PersonnelService } from '../../../services/personnel.service';
import { ReportService } from '../../../services/report.service';
import { RoomsService } from '../../../services/rooms.service';
import { SnackService } from '../../../services/snack.service';
import { TypeService } from '../../../services/type.service';
import { TEMSComponent } from '../../../tems/tems.component';
import { atLeastOne } from 'src/app/models/validators';

@Component({
  selector: 'app-create-report-template',
  templateUrl: './create-report-template.component.html',
  styleUrls: ['./create-report-template.component.scss']
})
export class CreateReportTemplateComponent extends TEMSComponent implements OnInit {

  updateReportId: string;
  templateToUpdate: AddReportTemplate;

  typesAlreadySelected: IOption[] = [];
  definitionsAlreadySelected: IOption[] = [];
  roomsAlreadySelected: IOption[] = [];
  personnelAlreadySelected: IOption[] = [];
  signatoriesAlreadySelected: IOption[] = [];

  typesEndPointParameter;

  reportFormGroup = new FormGroup({
    name: new FormControl(),
    description: new FormControl(),
    types: new FormControl(),
    definitions: new FormControl(),
    rooms: new FormControl(),
    personnel: new FormControl(),
    includeInUse: new FormControl(true),
    includeUnused: new FormControl(true),
    includeFunctional: new FormControl(true),
    includeDefect: new FormControl(true),
    includeParent: new FormControl(true),
    includeChildren: new FormControl(false),
    separateBy: new FormControl(),
    reportProperties: new FormControl(),
    header: new FormControl(),
    footer: new FormControl(),
    signatories: new FormControl()
  }, 
  { validators: [
    atLeastOne(Validators.requiredTrue, ['includeInUse', 'includeUnused']),
    atLeastOne(Validators.requiredTrue, ['includeFunctional', 'includeDefect']),
    atLeastOne(Validators.requiredTrue, ['includeParent', 'includeChildren']),
  ]});

  constructor(
    public roomService: RoomsService,
    public equipmentService: EquipmentService,
    public typeService: TypeService,
    public definitionService: DefinitionService,
    public personnelService: PersonnelService,
    private reportService: ReportService,
    private activatedroute: ActivatedRoute,
    public translate: TranslateService,
    private snackService: SnackService,
    private downloadService: DownloadService
  ) {
    super();
  }

  ngOnInit(): void {
    let controls = this.reportFormGroup.controls;
    controls.separateBy.setValue('none');

    if(this.updateReportId == undefined)
      this.updateReportId = this.activatedroute.snapshot.paramMap.get("id");
    
    if(this.updateReportId != undefined)
      this.edit();
  }

  edit(){
    this.subscriptions.push(
      this.reportService.getReportTemplateToUpdate(this.updateReportId)
      .subscribe(result => {
        this.templateToUpdate = result;
        if(this.templateToUpdate == null)
          return;

        console.log(this.templateToUpdate);

        let controls = this.reportFormGroup.controls;
        controls.name.setValue(this.templateToUpdate.name),
        controls.description.setValue(this.templateToUpdate.description),
        controls.types.setValue(this.templateToUpdate.types),
        controls.definitions.setValue(this.templateToUpdate.definitions),
        controls.rooms.setValue(this.templateToUpdate.rooms),
        controls.personnel.setValue(this.templateToUpdate.personnel),
        controls.separateBy.setValue(this.templateToUpdate.separateBy),
        controls.includeInUse.setValue(this.templateToUpdate.includeInUse),
        controls.includeUnused.setValue(this.templateToUpdate.includeUnused),
        controls.includeFunctional.setValue(this.templateToUpdate.includeFunctional),
        controls.includeDefect.setValue(this.templateToUpdate.includeDefect),
        controls.includeParent.setValue(this.templateToUpdate.includeParent),
        controls.includeChildren.setValue(this.templateToUpdate.includeChildren),
        controls.header.setValue(this.templateToUpdate.header),
        controls.footer.setValue(this.templateToUpdate.footer),
        controls.signatories.setValue(this.templateToUpdate.signatories),
        controls.name.setValue(this.templateToUpdate.name);

        if(this.templateToUpdate.types != undefined)
          this.typesAlreadySelected = this.templateToUpdate.types;
        
        if(this.templateToUpdate.definitions != undefined)
          this.definitionsAlreadySelected = this.templateToUpdate.definitions;
        
        if(this.templateToUpdate.rooms != undefined)
          this.roomsAlreadySelected = this.templateToUpdate.rooms;
        
        if(this.templateToUpdate.personnel != undefined)
          this.personnelAlreadySelected = this.templateToUpdate.personnel;
        
        if(this.templateToUpdate.signatories != undefined)
          this.signatoriesAlreadySelected = this.templateToUpdate.signatories.map(q => ({ label: q, value: q }));
      })
    );
  }

  notifyTypesChanged(){
    // workaround to notify other components when types changed
    this.reportFormGroup.controls.types.setValue([].concat(this.reportFormGroup.controls.types.value));
  }

  typeAdded() {
    this.notifyTypesChanged();

    this.typesEndPointParameter = this.reportFormGroup.controls.types.value == undefined
        ? null
        : this.reportFormGroup.controls.types.value.map(q => q.value);
    
    let validDefinitions = [];

    if(this.reportFormGroup.controls.definitions.value != null)
       validDefinitions = this.reportFormGroup.controls.definitions.value
       .filter(q1 => this.typesEndPointParameter.findIndex(q2 => q2 == q1.additional) > -1);

    this.definitionsAlreadySelected = validDefinitions;
    // this.findCommonAndSpecificProperties();
  }

  typeRemoved(eventData) {
    this.notifyTypesChanged();

    this.typesEndPointParameter = this.reportFormGroup.controls.types.value == undefined
        ? null
        : this.reportFormGroup.controls.types.value.map(q => q.value);
    
    let validDefinitions = [];

    if(this.reportFormGroup.controls.definitions.value != null)
       validDefinitions = this.reportFormGroup.controls.definitions.value
        .filter(q1 => this.typesEndPointParameter.findIndex(q2 => q2 == q1.additional) > -1);
       
    this.definitionsAlreadySelected = validDefinitions;
  }

  onCommonPropChange(eventData) {
    this.reportFormGroup.controls.commonProperties.setValue(eventData);
  }

  get controls(){
    return this.reportFormGroup.controls;
  }

  save() {
    console.log(this.reportFormGroup);
    
    let addReportTemplateModel = this.getReportTemplate();
    console.log(addReportTemplateModel);
    let endPoint = this.reportService.addReportTemplate(addReportTemplateModel);
    
    if(addReportTemplateModel.id != undefined)
      endPoint = this.reportService.updateReportTemplate(addReportTemplateModel); 

    this.subscriptions.push(
      endPoint
      .subscribe(result => {
        this.snackService.snack(result);
      })
    )
  }

  getReportTemplate(): AddReportTemplate{
    let addReportTemplate: AddReportTemplate = {
      id: this.updateReportId,
      name: this.controls.name.value,
      description: this.controls.description.value,
      types: this.controls.types.value,
      definitions: this.controls.definitions.value,
      rooms: this.controls.rooms.value,
      personnel: this.controls.personnel.value,
      separateBy: this.controls.separateBy.value,
      includeInUse: this.controls.includeInUse.value,
      includeUnused: this.controls.includeUnused.value,
      includeFunctional: this.controls.includeFunctional.value,
      includeDefect: this.controls.includeDefect.value,
      includeParent: this.controls.includeParent.value,
      includeChildren: this.controls.includeChildren.value,
      commonProperties: this.controls.reportProperties.value.commonProperties,
      specificProperties: (this.controls.reportProperties.value.typeSpecificProperties != undefined)
        ? this.controls.reportProperties.value.typeSpecificProperties
        : null,
      header: this.controls.header.value,
      footer: this.controls.footer.value,
      signatories: this.controls.signatories.value?.map(q => q.label),
      properties: undefined
    };

    console.log(addReportTemplate);

    return addReportTemplate;
  }

  generateReport(){
    let addReportTemplateModel = this.getReportTemplate();

    this.subscriptions.push(
      this.reportService.generateReportFromRawTemplate(addReportTemplateModel)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        this.downloadService.downloadFile(result, "Report.xlsx");
      })
    );
  }
}


// findCommonAndSpecificProperties() {
//   // 1. Put all common (that were not natively common) props back to where they belong (to their types)
//   // 2. Add or remove type specific properties
//   // 3. Find common properties
  
//   let types = this.reportFormGroup.controls.types.value;
  
//   if(types != undefined && types.length > 0)
//     this.putBackCommonProps();
//   else
//   {
//     this.equipmentCommonProperties = this.universalProperties;
//     this.equipmentCommonProperties.forEach(element => {
//       element.checked = false;
//       if(this.templateToUpdate.properties.indexOf(element.value) > -1)
//         element.checked = true;
//     });

//     return;
//   }

//   this.unsubscribeFromAll();

//   // Getting specific properties of selected types
//   for (let i = 0; i < types.length; i++) {
//     let element = this.reportFormGroup.controls.types.value[i];
//     if (this.typeSpecificProperties.find(q => q.type == element) == undefined) {
//       this.subscriptions.push(
//         this.typeService.getPropertiesOfType(element.value)
//           .subscribe(result => {
//             this.typeSpecificProperties.push(
//               {
//                 type: element,
//                 properties: result.map(q => new CheckboxItem(q.additional, q.label))
//               });

//             this.findCommonProps();
//           }
//         )
//       )
//     }
//   }
// }

// putBackCommonProps() {
//   // When there are multiple types selected, sometimes there might be common properties among those types.
//   // For example, both CPU and GPU have Name property. In this case, Name won't be placed in CPU and GPU
//   // selection group, but will be brought up to Common properites, due to the fact that It indeed is a common property
//   // along all of CURRENTLY SELECTED TYPES.

//   this.equipmentCommonProperties = this.equipmentCommonProperties
//     .filter((el) => !this.universalProperties.map(q => q.label).includes(el.label));

//   this.typeSpecificProperties.forEach(specific => {
//     this.equipmentCommonProperties.forEach(common => {
//       specific.properties.push(new CheckboxItem(common.value, common.label, common.checked))
//     });
//   });
// }

// findCommonProps() {
//   this.equipmentCommonProperties = this.universalProperties;
  
//   if (this.typeSpecificProperties.length >= 2){
//     let localCommonProperties = this.typeSpecificProperties[0].properties;

//     for (let i = 1; i < this.typeSpecificProperties.length; i++) {
//       localCommonProperties = localCommonProperties
//         .filter(value => this.typeSpecificProperties[i].properties
//           .map(q => q.label)
//           .includes(value.label));
//     }

//     this.typeSpecificProperties.forEach(element => {
//       element.properties = element.properties
//         .filter((value) => !localCommonProperties
//           .map(q => q.label)
//           .includes(value.label));
//     });

//     this.equipmentCommonProperties = this.equipmentCommonProperties
//       .concat(localCommonProperties);
//   }

//   // Marking necessary properties as checked
//   if(this.templateToUpdate == undefined)
//     return;

//   this.equipmentCommonProperties.forEach(element => {
//     element.checked = false;
//     if(this.templateToUpdate.properties.indexOf(element.value) > -1)
//       element.checked = true;
//   });

//   this.typeSpecificProperties.forEach(element =>{
//     element.properties.forEach(elementProperty =>{
//       if(this.templateToUpdate.properties.indexOf(elementProperty.value) > -1){
//         elementProperty.checked = true;
//       }
//     })

//     this.onSpecificPropChange(element.properties
//       .filter(q => q.checked)
//       .map(q => q.value), element.type);
//   })
// }

// onSpecificPropChange(eventData, type) {
//   let typeSpecific = this.specificProperties.find(q => q.type == type);
//   if (typeSpecific != undefined)
//     typeSpecific.properties = eventData;
//   else
//     this.specificProperties.push({ type: type, properties: eventData });
//   this.reportFormGroup.controls.specificProperties.setValue(this.specificProperties);
// }