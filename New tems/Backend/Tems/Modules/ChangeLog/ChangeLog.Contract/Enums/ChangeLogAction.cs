namespace ChangeLog.Contract.Enums;

public enum ChangeLogAction
{
    AssetCreated,
    AssetUpdated,
    AssetDeleted,
    AssetAssignedToUser,
    AssetUnassignedFromUser,
    AssetAssignedToLocation,
    AssetUnassignedFromLocation,
    AssetManualLogAdded,
    AssetArchived,
    AssetRestored,

    UserCreated,
    UserDeleted,
    UserRolesUpdated,
    UserAssetAssigned,
    UserAssetUnassigned,

    LocationCreated,
    LocationUpdated,
    LocationDeleted,
    LocationAssetAssigned,
    LocationAssetUnassigned
}
