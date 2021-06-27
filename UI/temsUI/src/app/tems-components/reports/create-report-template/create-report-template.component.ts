import { Downloader } from './../../../shared/downloader/fileDownloader';
import { SnackService } from '../../../services/snack.service';
import { filter } from 'rxjs/operators';
import { DefinitionService } from '../../../services/definition.service';
import { TypeService } from '../../../services/type.service';
import { ActivatedRoute } from '@angular/router';
import { ReportService } from '../../../services/report.service';
import { AddReportTemplate } from '../../../models/report/add-report.model';
import { TEMSComponent } from '../../../tems/tems.component';
import { CheckboxItem } from '../../../models/checkboxItem.model';
import { IOption } from 'src/app/models/option.model';
import { PersonnelService } from '../../../services/personnel.service';
import { EquipmentService } from '../../../services/equipment.service';
import { RoomsService } from '../../../services/rooms.service';
import { FormGroup, FormControl } from '@angular/forms';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-create-report-template',
  templateUrl: './create-report-template.component.html',
  styleUrls: ['./create-report-template.component.scss']
})
export class CreateReportTemplateComponent extends TEMSComponent implements OnInit {

  updateReportId: string;
  reportTemplateToUpdate: AddReportTemplate;
  downloader: Downloader;

  reportFormGroup: FormGroup;
  equipmentCommonProperties: CheckboxItem[];
  specificProperties: { type: IOption, properties: CheckboxItem[] }[] = [];
  universalProperties: CheckboxItem[];
  typeSpecificProperties: { type: IOption, properties: CheckboxItem[] }[] = [];

  reportObjectOptions = [
    { value: 'equipment', label: 'Equipment' },
    { value: 'rooms', label: 'Rooms (Not supported yet)' },
    { value: 'Personnel', label: 'Personnel (Not supported yet)' },
    { value: 'Allocations', label: 'Allocations (Not supported yet)' }
  ];

  typesAlreadySelected: IOption[] = [];
  definitionsAlreadySelected: IOption[] = [];
  roomsAlreadySelected: IOption[] = [];
  personnelAlreadySelected: IOption[] = [];
  signatoriesAlreadySelected: IOption[] = [];

  typesEndPointParameter;

  constructor(
    private roomService: RoomsService,
    private equipmentService: EquipmentService,
    private typeService: TypeService,
    private definitionService: DefinitionService,
    private personnelService: PersonnelService,
    private reportService: ReportService,
    private activatedroute: ActivatedRoute,
    private snackService: SnackService
  ) {
    super();
  }

  ngOnInit(): void {
    this.reportFormGroup = new FormGroup({
      name: new FormControl(),
      description: new FormControl(),
      subject: new FormControl(),
      types: new FormControl(),
      definitions: new FormControl(),
      rooms: new FormControl(),
      personnel: new FormControl(),
      separateBy: new FormControl(),
      commonProperties: new FormControl(),
      specificProperties: new FormControl(),
      header: new FormControl(),
      footer: new FormControl(),
      signatories: new FormControl()
    });

    let controls = this.reportFormGroup.controls;
    controls.subject.setValue('equipment'),
    controls.separateBy.setValue('none');

    this.universalProperties = [
      new CheckboxItem('temsid', 'TEMSID', true),
      new CheckboxItem('serialNumber', 'Serial Number', true),
      new CheckboxItem('definition', 'Definition', true),
      new CheckboxItem('type', 'Type', true),
      new CheckboxItem('description', 'Description', false),
      new CheckboxItem('price', 'Price', true),
      new CheckboxItem('currency', 'Currency', true, "HeheBoai"),
      new CheckboxItem('purchaseDate', 'Date of purchase', false),
      new CheckboxItem('allocatee', 'Allocatee'),
    ];
    this.equipmentCommonProperties = this.universalProperties;
    this.reportFormGroup.controls.commonProperties.setValue(this.equipmentCommonProperties.map(q => q.value));

    if(this.updateReportId == undefined)
      this.updateReportId = this.activatedroute.snapshot.paramMap.get("id");
    
    if(this.updateReportId != undefined)
      this.edit();
  }

  edit(){
    this.subscriptions.push(
      this.reportService.getReportTemplateToUpdate(this.updateReportId)
      .subscribe(result => {
        this.reportTemplateToUpdate = result;

        if(this.reportTemplateToUpdate == null)
          return;

        console.log('got this from server to update');
        console.log(this.reportTemplateToUpdate);

        let controls = this.reportFormGroup.controls;
        controls.name.setValue(this.reportTemplateToUpdate.name),
        controls.description.setValue(this.reportTemplateToUpdate.description),
        controls.subject.setValue(this.reportTemplateToUpdate.subject),
        controls.types.setValue(this.reportTemplateToUpdate.types),
        controls.definitions.setValue(this.reportTemplateToUpdate.definitions),
        controls.rooms.setValue(this.reportTemplateToUpdate.rooms),
        controls.personnel.setValue(this.reportTemplateToUpdate.personnel),
        controls.separateBy.setValue(this.reportTemplateToUpdate.separateBy),
        controls.header.setValue(this.reportTemplateToUpdate.header),
        controls.footer.setValue(this.reportTemplateToUpdate.footer),
        controls.signatories.setValue(this.reportTemplateToUpdate.signatories),
        controls.name.setValue(this.reportTemplateToUpdate.name);

        if(this.reportTemplateToUpdate.types != undefined)
          this.typesAlreadySelected = this.reportTemplateToUpdate.types;
        if(this.reportTemplateToUpdate.definitions != undefined)
          this.definitionsAlreadySelected = this.reportTemplateToUpdate.definitions;
        if(this.reportTemplateToUpdate.rooms != undefined)
          this.roomsAlreadySelected = this.reportTemplateToUpdate.rooms;
        if(this.reportTemplateToUpdate.personnel != undefined)
          this.personnelAlreadySelected = this.reportTemplateToUpdate.personnel;
        if(this.reportTemplateToUpdate.signatories != undefined)
          this.signatoriesAlreadySelected = this.reportTemplateToUpdate.signatories;

        this.findCommonAndSpecificProperties();      
      })
    )
  }

  typeAdded(eventData) {
    this.typesEndPointParameter = this.reportFormGroup.controls.types.value == undefined
        ? null
        : this.reportFormGroup.controls.types.value.map(q => q.value);
    
    let validDefinitions = [];

    if(this.reportFormGroup.controls.definitions.value != null)
       validDefinitions = this.reportFormGroup.controls.definitions.value
       .filter(q1 => this.typesEndPointParameter.findIndex(q2 => q2 == q1.additional) > -1);

        console.log('valid');
        console.log(validDefinitions);

    this.definitionsAlreadySelected = validDefinitions;

    this.findCommonAndSpecificProperties();
  }

  typeRemoved(eventData) {
    this.typesEndPointParameter = this.reportFormGroup.controls.types.value == undefined
        ? null
        : this.reportFormGroup.controls.types.value.map(q => q.value);
    
    let validDefinitions = [];

    if(this.reportFormGroup.controls.definitions.value != null)
       validDefinitions = this.reportFormGroup.controls.definitions.value
        .filter(q1 => this.typesEndPointParameter.findIndex(q2 => q2 == q1.additional) > -1);
       
        console.log(validDefinitions);

    this.definitionsAlreadySelected = validDefinitions;

    console.log('event data:');
    let index = this.typeSpecificProperties.findIndex(q => q.type == eventData);
    if (index > -1) {
      console.log('typespecific removed');
      this.typeSpecificProperties.splice(index, 1);
    }

    index = this.specificProperties.findIndex(q => q.type == eventData);
    if(index > -1){
      console.log('specific property removed');
      this.specificProperties.splice(index, 1);
  
      this.reportFormGroup.controls.specificProperties.setValue(this.specificProperties);
    }

    this.putBackCommonProps();
    this.findCommonProps();
  }

  onCommonPropChange(eventData) {
    this.reportFormGroup.controls.commonProperties.setValue(eventData);
  }

  findCommonAndSpecificProperties() {
    // 1. Put all common props back to where they belong
    // 2. Add or remove type specific properties
    // 3. Find common properties

    this.putBackCommonProps();
    this.unsubscribeFromAll();
    // Getting specific properties of selected types
    let length = this.reportFormGroup.controls.types.value.length;
    for (let i = 0; i < length; i++) {
      let element = this.reportFormGroup.controls.types.value[i];
      if (this.typeSpecificProperties.find(q => q.type == element) == undefined) {
        this.subscriptions.push(
          this.equipmentService.getPropertiesOfType(element.value)
            .subscribe(result => {
              console.log('ELEMENT:');
              console.log(element);

              this.typeSpecificProperties.push(
                {
                  type: element,
                  properties: result.map(q => new CheckboxItem(q.additional, q.label))
                });

              this.findCommonProps();
            }
          )
        )
      }
    }
  }

  putBackCommonProps() {
    this.equipmentCommonProperties = this.equipmentCommonProperties
      .filter((el) => !this.universalProperties.map(q => q.label).includes(el.label));

    this.typeSpecificProperties.forEach(specific => {
      this.equipmentCommonProperties.forEach(common => {
        specific.properties.push(new CheckboxItem(common.value, common.label, common.checked))
      });
    });
  }

  findCommonProps() {
    this.equipmentCommonProperties = this.universalProperties;
    
    if (this.typeSpecificProperties.length >= 2){
      let localCommonProperties = this.typeSpecificProperties[0].properties;

      for (let i = 1; i < this.typeSpecificProperties.length; i++) {
        localCommonProperties = localCommonProperties
          .filter(value => this.typeSpecificProperties[i].properties
            .map(q => q.label)
            .includes(value.label));
      }
  
      this.typeSpecificProperties.forEach(element => {
        element.properties = element.properties
          .filter((value) => !localCommonProperties
            .map(q => q.label)
            .includes(value.label));
      });
  
      this.equipmentCommonProperties = this.equipmentCommonProperties.concat(localCommonProperties);
    }

    // Marking necessary properties as checked
    if(this.reportTemplateToUpdate == undefined)
      return;
  
    this.equipmentCommonProperties.forEach(element => {
      element.checked = false;
      if(this.reportTemplateToUpdate.properties.indexOf(element.value) > -1)
        element.checked = true;
      
      console.log(element.value + ' ' + element.checked)
    });

    this.typeSpecificProperties.forEach(element =>{
      element.properties.forEach(elementProperty =>{
        if(this.reportTemplateToUpdate.properties.indexOf(elementProperty.value) > -1){
          elementProperty.checked = true;
        }
      })

      console.log('adding');
      console.log(element.properties.filter(q => q.checked));
      console.log('to')
      console.log(element.type);
      this.onSpecificPropChange(element.properties.filter(q => q.checked).map(q => q.value), element.type);
    })
  }

  onSpecificPropChange(eventData, type) {
    let typeSpecific = this.specificProperties.find(q => q.type == type);
    if (typeSpecific != undefined)
      typeSpecific.properties = eventData;
    else
      this.specificProperties.push({ type: type, properties: eventData });
    this.reportFormGroup.controls.specificProperties.setValue(this.specificProperties);
  }

  get controls(){
    return this.reportFormGroup.controls;
  }

  save(generateReportAfterSaving: boolean = false) {
    let addReportTemplateModel = this.getReportTemplate();
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
      subject: this.controls.subject.value,
      types: this.controls.types.value,
      definitions: this.controls.definitions.value,
      rooms: this.controls.rooms.value,
      personnel: this.controls.personnel.value,
      separateBy: this.controls.separateBy.value,
      commonProperties: this.controls.commonProperties.value,
      specificProperties: (this.controls.specificProperties.value != null)
        ? this.controls.specificProperties.value.map(q => ({properties: q.properties, type: q.type.value}))
        : null,
      header: this.controls.header.value,
      footer: this.controls.footer.value,
      signatories: this.controls.signatories.value,
      properties: undefined
    };

    return addReportTemplate;
  }

  generateReport(){
    let addReportTemplateModel = this.getReportTemplate();

    this.subscriptions.push(
      this.reportService.generateReportFromRawTemplate(addReportTemplateModel)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        if(this.downloader == undefined) this.downloader = new Downloader();
        this.downloader.downloadFile(result, "Report.xlsx");
      })
    )
  }
}
