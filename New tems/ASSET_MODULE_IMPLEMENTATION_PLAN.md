# Asset Module Frontend Implementation Plan

## Date: 3 January 2026

## Overview
Implement a comprehensive asset management system in the frontend with two main sections:
1. **Assets** - View and manage asset instances (user-facing)
2. **Asset Management (Admin)** - Configure asset properties, types, and definitions (admin-facing)

## Backend Endpoints Available
- `/asset-property` - Property catalog CRUD
- `/asset-type` - Asset type hierarchy CRUD
- `/asset-definition` - Asset definition templates CRUD
- `/asset` - Asset instances CRUD

## Changes Required

### 1. Menu/Sidebar Updates (menu.service.ts)
**Remove:**
- Issues section (lines ~233-248)
- Communication section (lines ~249-273)

**Add Before Technical Support:**
```typescript
{
  path: '',
  title: 'Assets',
  icon: 'mdi mdi-package-variant menu-icon',
  isActive: false,
  isShown: true,
  showSubmenu: false,
  submenu: [
    {
      path: '/assets/view',
      title: 'Assets',
      icon: 'mdi mdi-view-list menu-icon',
      isActive: false,
      isShown: true,
      showSubmenu: false,
      submenu: []
    },
    {
      path: '/assets/management',
      title: 'Asset Management',
      icon: 'mdi mdi-cog menu-icon',
      isActive: false,
      isShown: this.tokenService.canManageEntities(),
      showSubmenu: false,
      submenu: []
    }
  ]
}
```

### 2. Routing Configuration (app-routing.module.ts)
**Add after Technical Support section:**
```typescript
{
  path: 'assets',
  children: [
    { path: '', redirectTo: 'view', pathMatch: 'full' },
    { path: 'view', loadComponent: () => import('./tems-components/asset-module/view-assets/view-assets.component').then(m => m.ViewAssetsComponent) },
    { path: 'management', loadComponent: () => import('./tems-components/asset-module/asset-management/asset-management.component').then(m => m.AssetManagementComponent), canActivate: [CanManageEntitiesGuard] },
    { path: ':id', loadComponent: () => import('./tems-components/asset-module/asset-detail/asset-detail.component').then(m => m.AssetDetailComponent) }
  ]
}
```

### 3. Components to Create

#### A. View Assets Component (User-Facing)
**Path:** `src/app/tems-components/asset-module/view-assets/`
**Purpose:** Display all asset instances with filtering, search, and detail view
**Design Pattern:** Similar to `view-tickets.component.ts`
**Features:**
- AG Grid table with columns: Asset Tag, Serial Number, Type, Status, Location, Assigned To
- Search/Filter functionality
- Row click to view details
- Export functionality
- Status badges (Available, In Use, Under Maintenance, Retired)

#### B. Asset Detail Component
**Path:** `src/app/tems-components/asset-module/asset-detail/`
**Purpose:** Display detailed view of a single asset
**Design Pattern:** Similar to `ticket-detail.component.ts`
**Features:**
- Asset information panel
- Purchase information
- Location history
- Assignment history
- Maintenance records
- Specifications from embedded definition snapshot

#### C. Asset Management Component (Admin)
**Path:** `src/app/tems-components/asset-module/asset-management/`
**Purpose:** Tabbed interface for managing properties, types, and definitions
**Design Pattern:** Similar to `view-ticket-types.component.ts` but with horizontal tabs
**Features:**
- Horizontal iOS-style tabs (Properties | Types | Definitions)
- Tab 1: Property Management (asset-property-management component)
- Tab 2: Type Management (asset-type-management component)
- Tab 3: Definition Management (asset-definition-management component)

#### D. Asset Property Management Component
**Path:** `src/app/tems-components/asset-module/asset-management/asset-property-management/`
**Purpose:** CRUD for property catalog
**Features:**
- AG Grid with columns: Name, Category, Data Type, Unit, Enum Values
- Create/Edit/Delete modals
- Validation rules editor

#### E. Asset Type Management Component
**Path:** `src/app/tems-components/asset-module/asset-management/asset-type-management/`
**Purpose:** CRUD for asset types with hierarchy support
**Features:**
- AG Grid with columns: Name, Parent Type, Properties Count, Is Archived
- Hierarchical tree view (optional)
- Property selector for type-specific properties
- Create/Edit/Delete modals

#### F. Asset Definition Management Component
**Path:** `src/app/tems-components/asset-module/asset-management/asset-definition-management/`
**Purpose:** CRUD for asset definition templates
**Features:**
- AG Grid with columns: Name, Type, Manufacturer, Model, Usage Count
- Specifications editor (key-value pairs)
- Tags management
- Create/Edit/Delete modals

**Current implementation status (Jan 2026):**
- AddDefinition dialog now uses a reactive form with specification rows (property, value, data type, unit) and supports both create and update flows with type locking when launched from a specific type.
- Asset Definition Management grid opens the shared AddDefinition dialog for create/edit; the legacy inline edit modal has been removed.

### 4. Services to Create

#### A. Asset Service
**Path:** `src/app/services/asset.service.ts`
**Methods:**
- `getAll(): Observable<Asset[]>`
- `getById(id: string): Observable<Asset>`
- `getByAssetTag(tag: string): Observable<Asset>`
- `getBySerialNumber(sn: string): Observable<Asset>`
- `create(asset: CreateAssetRequest): Observable<Asset>`
- `update(id: string, asset: UpdateAssetRequest): Observable<Asset>`
- `delete(id: string): Observable<void>`
- `archive(id: string): Observable<void>`

#### B. Asset Property Service
**Path:** `src/app/services/asset-property.service.ts`
**Methods:**
- `getAll(): Observable<AssetProperty[]>`
- `getById(id: string): Observable<AssetProperty>`
- `create(property: CreateAssetPropertyRequest): Observable<AssetProperty>`
- `update(id: string, property: UpdateAssetPropertyRequest): Observable<AssetProperty>`
- `delete(id: string): Observable<void>`

#### C. Asset Type Service
**Path:** `src/app/services/asset-type.service.ts`
**Methods:**
- `getAll(): Observable<AssetType[]>`
- `getById(id: string): Observable<AssetType>`
- `create(type: CreateAssetTypeRequest): Observable<AssetType>`
- `update(id: string, type: UpdateAssetTypeRequest): Observable<AssetType>`
- `delete(id: string): Observable<void>`
- `archive(id: string): Observable<void>`

#### D. Asset Definition Service
**Path:** `src/app/services/asset-definition.service.ts`
**Methods:**
- `getAll(): Observable<AssetDefinition[]>`
- `getById(id: string): Observable<AssetDefinition>`
- `create(def: CreateAssetDefinitionRequest): Observable<AssetDefinition>`
- `update(id: string, def: UpdateAssetDefinitionRequest): Observable<AssetDefinition>`
- `delete(id: string): Observable<void>`
- `archive(id: string): Observable<void>`

### 5. Models to Create

#### A. Asset Models
**Path:** `src/app/models/asset/asset.model.ts`
```typescript
export interface Asset {
  id: string;
  assetTag: string;
  serialNumber?: string;
  status: AssetStatus;
  assetTypeId: string;
  assetTypeName: string;
  definition: AssetDefinitionSnapshot;
  purchaseInfo?: PurchaseInfo;
  location?: AssetLocation;
  assignment?: AssetAssignment;
  parentAssetId?: string;
  childAssetIds: string[];
  maintenanceHistory: MaintenanceRecord[];
  customFields?: { [key: string]: any };
  notes?: string;
  isArchived: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export interface AssetDefinitionSnapshot {
  definitionId: string;
  name: string;
  manufacturer?: string;
  model?: string;
  specifications: { [key: string]: string };
  snapshotTakenAt: Date;
}

export interface PurchaseInfo {
  vendor?: string;
  purchaseDate?: Date;
  purchasePrice?: number;
  warrantyExpiration?: Date;
  invoiceNumber?: string;
}

export interface AssetLocation {
  building?: string;
  floor?: string;
  room?: string;
  updatedAt: Date;
}

export interface AssetAssignment {
  assignedToUserId?: string;
  assignedToUserName?: string;
  assignedAt?: Date;
  dueDate?: Date;
}

export interface MaintenanceRecord {
  date: Date;
  description: string;
  performedBy?: string;
  cost?: number;
}

export enum AssetStatus {
  Available = 'AVAILABLE',
  InUse = 'IN_USE',
  UnderMaintenance = 'UNDER_MAINTENANCE',
  Retired = 'RETIRED'
}
```

#### B. Property Models
**Path:** `src/app/models/asset/asset-property.model.ts`
```typescript
export interface AssetProperty {
  id: string;
  name: string;
  category: string;
  dataType: PropertyDataType;
  enumValues?: string[];
  unit?: string;
  defaultValidation?: PropertyValidation;
  description?: string;
  createdBy?: string;
  createdAt: Date;
  updatedAt: Date;
}

export interface PropertyValidation {
  type: string;
  maxLength?: number;
  pattern?: string;
  min?: number;
  max?: number;
  unit?: string;
  enumValues?: string[];
}

export enum PropertyDataType {
  String = 'STRING',
  Number = 'NUMBER',
  Boolean = 'BOOLEAN',
  Date = 'DATE',
  Enum = 'ENUM'
}
```

#### C. Type Models
**Path:** `src/app/models/asset/asset-type.model.ts`
```typescript
export interface AssetType {
  id: string;
  name: string;
  description?: string;
  parentTypeId?: string;
  parentTypeName?: string;
  properties: AssetTypeProperty[];
  isArchived: boolean;
  archivedAt?: Date;
  archivedBy?: string;
  createdAt: Date;
  updatedAt: Date;
}

export interface AssetTypeProperty {
  propertyId: string;
  propertyName: string;
  isRequired: boolean;
  displayOrder: number;
  defaultValue?: string;
}
```

#### D. Definition Models
**Path:** `src/app/models/asset/asset-definition.model.ts`
```typescript
export interface AssetDefinition {
  id: string;
  assetTypeId: string;
  assetTypeName: string;
  name: string;
  manufacturer?: string;
  model?: string;
  specifications: { [key: string]: string };
  usageCount: number;
  tags: string[];
  isArchived: boolean;
  archivedAt?: Date;
  archivedBy?: string;
  createdAt: Date;
  updatedAt: Date;
}
```

### 6. State Management (Optional but Recommended)
**Path:** `src/app/state/asset-management.state.ts`
Similar to `ticket-management.state.ts` for reactive state handling

### 7. Backend Configuration
**Path:** `src/app/config/backend.config.ts`
Add API endpoints:
```typescript
export const API_ASSET_URL = `${API_BASE_URL}/asset`;
export const API_ASSET_PROPERTY_URL = `${API_BASE_URL}/asset-property`;
export const API_ASSET_TYPE_URL = `${API_BASE_URL}/asset-type`;
export const API_ASSET_DEFINITION_URL = `${API_BASE_URL}/asset-definition`;
```

### 8. End-to-End Tests

#### Test Files to Create:
1. `assets.e2e.spec.ts` - Test asset viewing and filtering
2. `asset-management.e2e.spec.ts` - Test admin CRUD operations
3. `asset-property-management.e2e.spec.ts` - Test property management
4. `asset-type-management.e2e.spec.ts` - Test type management
5. `asset-definition-management.e2e.spec.ts` - Test definition management

#### Test Scenarios:
- **Assets View:**
  - Load assets list
  - Filter by status
  - Search by asset tag/serial number
  - Click to view details
  
- **Property Management:**
  - Create property with validation
  - Edit property
  - Delete property
  - Validate enum values for ENUM type
  
- **Type Management:**
  - Create type without parent
  - Create child type with parent
  - Add properties to type
  - Archive type
  
- **Definition Management:**
  - Create definition for a type
  - Add specifications
  - Edit definition
  - Verify usage count increments when asset created

## Implementation Order

1. ✅ Create documentation (this file)
2. Create models (all interfaces)
3. Create services (HTTP calls to backend)
4. Update backend.config.ts with API URLs
5. Create Asset Management component with tabs
6. Create Property Management sub-component
7. Create Type Management sub-component
8. Create Definition Management sub-component
9. Create View Assets component
10. Create Asset Detail component
11. Update menu.service.ts (remove Issues/Communication, add Assets)
12. Update app-routing.module.ts (add assets routes)
13. Write E2E tests
14. Run all tests and fix issues
15. Manual testing and UI polish

## Design Consistency Guidelines

### Colors & Styling (From Ticket Components)
- **Action Buttons:** Blue primary (`text-blue-600 hover:text-blue-800`)
- **Delete Buttons:** Red (`text-red-600 hover:text-red-800`)
- **Status Badges:**
  - Active/Available: Green (`text-green-600`)
  - Inactive/Retired: Gray (`text-gray-600`)
  - Critical/High: Red/Orange (`text-red-600`, `text-orange-600`)
  - Medium: Yellow (`text-yellow-600`)
- **Modal Backdrop:** `bg-black/50`
- **Card Backgrounds:** `bg-white rounded-lg shadow-md`
- **Input Fields:** Tailwind form classes with focus states

### Grid Configuration
- Use AG Grid with similar column definitions
- Default sorting and filtering enabled
- Resizable columns
- Action buttons in last column
- Date formatting consistent across all grids

### Modal Patterns
- Fixed positioning with backdrop
- iOS-style rounded corners
- Smooth transitions
- Close on backdrop click
- Form validation with error messages

### Tab Design (iOS-style)
- Horizontal tabs at top of page
- Active tab: Blue bottom border (`border-b-2 border-blue-600`)
- Inactive tabs: Gray (`text-gray-600 hover:text-gray-900`)
- Smooth tab transitions
- Content area below tabs with padding

## Validation & Error Handling
- Form validation before API calls
- Loading states during async operations
- Error messages for failed API calls
- Success notifications after CRUD operations
- Confirmation dialogs for delete operations

## Notes
- All components use standalone: true
- All services use HttpClient with proper typing
- Follow existing patterns from ticket-management module
- Ensure responsive design (works on mobile/tablet)
- Add proper accessibility attributes (ARIA labels)
- Use proper TypeScript typing throughout
- No console.log in production code
- Comment complex logic only if absolutely necessary

## Potential Issues & Solutions

### Issue: Definition snapshot complexity
**Solution:** Create dedicated snapshot mapper service to handle embedding

### Issue: Hierarchical type display
**Solution:** Start with flat list, add tree view as enhancement

### Issue: Property validation in forms
**Solution:** Dynamic form generation based on property definitions

### Issue: Large datasets in grids
**Solution:** Implement server-side pagination if needed

## Success Criteria
- [ ] All CRUD operations work for all 4 entity types
- [ ] Assets display with proper filtering
- [ ] Asset Management tabs work smoothly
- [ ] No console errors
- [ ] All E2E tests pass
- [ ] UI matches existing ticket management design
- [ ] Responsive on all screen sizes
- [ ] Backend integration successful
- [ ] Issues and Communication sections removed from sidebar
- [ ] Assets menu appears above Technical Support
