import { AddDefinition } from './../../../models/equipment/add-definition.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Component, OnInit, Input, Inject } from '@angular/core';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { FormlyParserService } from 'src/app/services/formly-parser-service/formly-parser.service';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-add-definition',
  templateUrl: './add-definition.component.html',
  styleUrls: ['./add-definition.component.scss']
})
export class AddDefinitionComponent implements OnInit {

  constructor(
    private formlyParserService : FormlyParserService,
    private equipmentService: EquipmentService,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef?: MatDialogRef<AddDefinitionComponent>) {
  }


  private formlyData = {
    isVisible: false,
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  ngOnInit(): void {
    let parentFullType = this.equipmentService.getFullType(this.data.selectedType.id);
    
    let addDefinition = new AddDefinition();
    addDefinition.equipmentType = parentFullType;
    
    // Properties are copied from the definition types because we don't want
    // definition properties to be tight coupled to equipment properties.
    // We may add / remove properties from a definition (this support will be added soon)
    addDefinition.properties = addDefinition.equipmentType.properties;

    if(parentFullType.children != undefined)
      parentFullType.children.forEach(childType => {
        let childDefinition = new AddDefinition();
        childDefinition.equipmentType = childType;
        childDefinition.properties = childType.properties;
        
        addDefinition.children.push(childDefinition);
      });

    this.formlyData.model ={};
    this.formlyData.fields = this.formlyParserService.parseAddDefinition(addDefinition);
   
    // this.formlyData.fields = [
    //   {
    //     key: 'customer',
    //     fieldGroup: [
    //       {
    //         key: 'id',
    //         type: 'input',
    //         defaultValue: '100',
    //         templateOptions: {
    //           required: true,
    //           label: 'Id'
    //         }
    //       },
    //       {
    //         key: 'name',
    //         type: 'input',
    //         defaultValue: 'Test Customer',
    //         templateOptions: {
    //           required: true,
    //           label: 'Customer Name'
    //         }
    //       },
    //       {
    //         key: 'investments',
    //         type: 'repeat',
    //         fieldArray: {
    //           fieldGroupClassName: 'row',
    //           templateOptions: {
    //             btnText: 'Add another investment',
    //           },
    //           fieldGroup: [
    //             {
    //               className: 'col-sm-4',
    //               type: 'input',
    //               key: 'customerId',
    //               templateOptions: {
    //                 label: 'Customer Id:',
    //                 required: true,
    //               },
    //               expressionProperties: {
    //                 'model.customerId': 'formState.model.customer.id',
    //               },
    //             },
    //             {
    //               className: 'col-sm-4',
    //               type: 'input',
    //               key: 'investmentName',
    //               defaultValue: 'My name',
    //               templateOptions: {
    //                 label: 'Name of Investment:',
    //                 required: true,
    //               },
    //             },
    //             {
    //               type: 'input',
    //               key: 'investmentDate',
    //               className: 'col-sm-3',
    //               templateOptions: {
    //                 type: 'date',
    //                 label: 'Date of Investment:',
    //               },
    //             },
    //             {
    //               type: 'input',
    //               key: 'stockIdentifier',
    //               className: 'col-sm-3',
    //               defaultValue: 'test',
    //               templateOptions: {
    //                 label: 'Stock Identifier:',
    //                 addonRight: {
    //                   class: 'fa fa-code',
    //                   onClick: (to, fieldType, $event) => console.log(to, fieldType, $event),
    //                 },
    //               },
    //             },
    //           ],
    //         },
    //       },  
    //     ]
    //   }
    // ];  
    this.formlyData.isVisible = true;
  }

  onSubmit(model){
    // Send data to API
    console.log('submitted');
    console.log(model);

    // setInterval(() => { console.log(this.formlyData.fields)}, 4000)
  }
}
