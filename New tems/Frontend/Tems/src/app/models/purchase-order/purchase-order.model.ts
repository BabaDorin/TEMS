export interface PurchaseOrderItem {
  assetId: string;
  assetTag: string;
  serialNumber: string;
  price: number;
  createdAt: string;
}

export interface PurchaseOrder {
  id: string;
  ticketId: string;
  ticketHumanReadableId: string;
  poNumber: string;
  vendor: string;
  amount: number;
  currency: string;
  description: string;
  createdByUserId: string;
  createdByDisplayName: string;
  accountableUserId: string;
  accountableDisplayName: string;
  usedAmount: number;
  availableAmount: number;
  itemCount: number;
  items: PurchaseOrderItem[];
  createdAt: string;
  updatedAt: string;
}

export interface GetAllPurchaseOrdersResponse {
  purchaseOrders: PurchaseOrder[];
}

export interface GetPurchaseOrderByIdResponse {
  purchaseOrder?: PurchaseOrder | null;
}

export interface DeletePurchaseOrderResponse {
  success: boolean;
}
