export interface Ticket {
  ticketId: string;
  tenantId: string;
  ticketTypeId: string;
  humanReadableId: string;
  title: string;
  summary: string;
  aiSummary?: string;
  currentStateId: string;
  priority: 'LOW' | 'MEDIUM' | 'HIGH' | 'CRITICAL';
  reporter: Reporter;
  accountableUserId: string;
  accountableDisplayName?: string | null;
  assigneeId?: string;
  attributes: { [key: string]: any };
  approvalGates?: ApprovalGate[];
  createdAt: string;
  updatedAt: string;
  resolvedAt?: string | null;
  auditMetadata?: AuditMetadata;
  assetIds?: string[];
  linkedAssets?: LinkedAsset[];
}

export interface LinkedAsset {
  assetId: string;
  assetTag: string;
}

export interface Reporter {
  userId: string;
  channelSource: 'TEAMS' | 'SLACK' | 'WEB';
  channelThreadId?: string;
  displayName?: string | null;
}

export interface AuditMetadata {
  createdAt: string | Date;
  updatedAt: string | Date;
  resolvedAt?: string | Date;
}

export interface CreateTicketRequest {
  ticketTypeId: string;
  title: string;
  summary: string;
  priority: string;
  reporter: Reporter;
  accountableUserId?: string;
  assigneeId?: string;
  attributes: { [key: string]: any };
  assetIds?: string[];
}

export interface UpdateTicketRequest {
  summary?: string;
  currentStateId?: string;
  priority?: string;
  assigneeId?: string;
  attributes?: { [key: string]: any };
  assetIds?: string[];
}

export interface ApprovalGate {
  approvalGateId: string;
  title: string;
  justification: string;
  state: 'approved' | 'pending' | 'not-approved' | string;
  allApproversRequired: boolean;
  approvers: ApprovalGateApprover[];
  createdAt: string;
  updatedAt: string;
}

export interface ApprovalGateApprover {
  userId: string;
  status: 'approved' | 'pending' | 'rejected' | string;
  reviewedAt?: string | null;
}

export interface ApprovalGateRequest {
  title: string;
  justification: string;
  allApproversRequired: boolean;
  approverUserIds: string[];
}

export interface ApprovalGateResponse {
  success: boolean;
  gate?: ApprovalGate;
}

export interface ReviewApprovalGateRequest {
  status: 'approved' | 'rejected';
}

export interface ReviewApprovalGateResponse {
  success: boolean;
  gate?: ApprovalGate;
}

export interface TicketMessage {
  messageId?: string;
  senderType: 'USER' | 'AGENT' | 'AI_SYSTEM';
  senderId: string;
  timestamp: Date | string;
  content: string;
  channelMessageId?: string;
  isInternalNote: boolean;
  editedAt?: Date | string | null;
}

export interface TicketConversation {
  conversationId: string;
  ticketId: string;
  messages: TicketMessage[];
}

export interface AddMessageRequest {
  senderType: string;
  senderId: string;
  content: string;
  isInternalNote: boolean;
}

export interface AddMessageResponse {
  success: boolean;
  message?: TicketMessage;
}

export interface EditMessageResponse {
  success: boolean;
  message?: TicketMessage;
}
