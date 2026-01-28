export enum RoomType {
  Meeting = 'MEETING',
  Desk = 'DESK',
  Workshop = 'WORKSHOP',
  ServerRoom = 'SERVER_ROOM'
}

export enum RoomStatus {
  Available = 'AVAILABLE',
  Maintenance = 'MAINTENANCE',
  Decommissioned = 'DECOMMISSIONED'
}

export interface Room {
  id: string;
  buildingId: string;
  name: string;
  roomNumber?: string;
  floorLabel?: string;
  description?: string;
  type: RoomType;
  capacity?: number;
  area?: number;
  status: RoomStatus;
  createdAt: Date;
  updatedAt: Date;
  createdBy?: string;
  assetCounts?: Record<string, number>;
}

export interface RoomWithHierarchy extends Room {
  siteName?: string;
  siteId?: string;
  buildingName?: string;
}
