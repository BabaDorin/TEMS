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
  assigneeId?: string;
  attributes: { [key: string]: any };
  createdAt: string;
  updatedAt: string;
  resolvedAt?: string | null;
  auditMetadata?: AuditMetadata;
  assetIds?: string[];
}

export interface Reporter {
  userId: string;
  channelSource: 'TEAMS' | 'SLACK' | 'WEB';
  channelThreadId?: string;
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
