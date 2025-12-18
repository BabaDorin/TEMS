export interface Ticket {
  ticketId: string;
  tenantId: string;
  ticketTypeId: string;
  humanReadableId: string;
  summary: string;
  currentStateId: string;
  priority: 'LOW' | 'MEDIUM' | 'HIGH' | 'CRITICAL';
  reporter: Reporter;
  assigneeId?: string;
  attributes: { [key: string]: any };
  auditMetadata: AuditMetadata;
  equipmentIds?: string[];
}

export interface Reporter {
  userId: string;
  channelSource: 'TEAMS' | 'SLACK' | 'WEB';
  channelThreadId?: string;
}

export interface AuditMetadata {
  createdAt: Date;
  updatedAt: Date;
  resolvedAt?: Date;
}

export interface CreateTicketRequest {
  ticketTypeId: string;
  summary: string;
  priority: string;
  reporter: Reporter;
  assigneeId?: string;
  attributes: { [key: string]: any };
  equipmentIds?: string[];
}

export interface UpdateTicketRequest {
  summary?: string;
  currentStateId?: string;
  priority?: string;
  assigneeId?: string;
  attributes?: { [key: string]: any };
}

export interface TicketMessage {
  messageId: string;
  senderType: 'USER' | 'AGENT' | 'AI_SYSTEM';
  senderId: string;
  timestamp: Date;
  content: string;
  channelMessageId?: string;
  isInternalNote: boolean;
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
