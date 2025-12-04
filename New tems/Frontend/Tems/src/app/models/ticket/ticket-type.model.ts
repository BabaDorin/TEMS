export interface TicketType {
  ticketTypeId: string;
  tenantId: string;
  name: string;
  description: string;
  itilCategory: 'INCIDENT' | 'PROBLEM' | 'CHANGE' | 'REQUEST' | 'SECURITY_INCIDENT' | 'ALERT';
  version: number;
  workflowConfig: WorkflowConfig;
  attributeDefinitions: AttributeDefinition[];
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export interface WorkflowConfig {
  states: WorkflowState[];
  initialStateId: string;
}

export interface WorkflowState {
  id: string;
  label: string;
  type: 'OPEN' | 'WIP' | 'PENDING' | 'RESOLVED' | 'CLOSED';
  allowedTransitions: string[];
  automationHook?: string;
}

export interface AttributeDefinition {
  key: string;
  label: string;
  dataType: 'STRING' | 'NUMBER' | 'BOOL' | 'DATE' | 'USER' | 'ASSET_REF';
  isRequired: boolean;
  aiExtractionHint?: string;
  validationRule?: string;
}

export interface CreateTicketTypeRequest {
  name: string;
  description: string;
  itilCategory: string;
  workflowConfig: WorkflowConfig;
  attributeDefinitions: AttributeDefinition[];
}

export interface UpdateTicketTypeRequest {
  name?: string;
  description?: string;
  isActive?: boolean;
}
