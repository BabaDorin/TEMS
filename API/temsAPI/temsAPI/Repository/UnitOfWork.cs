﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.KeyEntities;
using temsAPI.Data.Entities.LibraryEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.Report;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly ApplicationDbContext _context;

        private IGenericRepository<Announcement> _announcements;
        private IGenericRepository<FrequentTicketProblem> _frequentTicketProblems;
        private IGenericRepository<Log> _logs;
        private IGenericRepository<Ticket> _tickets;
        private IGenericRepository<Equipment> _equipments;
        private IGenericRepository<EquipmentDefinition> _equipmentDefinitions;
        private IGenericRepository<EquipmentSpecifications> _equipmentSpecifications;
        private IGenericRepository<EquipmentType> _equipmentTypes;
        private IGenericRepository<Property> _properties;
        private IGenericRepository<DataType> _dataTypes;
        private IGenericRepository<PropertyEquipmentTypeAssociation> _propertyEquipmentTypeAssociations;
        private IGenericRepository<Key> _keys;
        private IGenericRepository<KeyAllocation> _keyAllocations;
        private IGenericRepository<TEMSUser> _temsUsers;
        private IGenericRepository<IdentityRole> _roles;
        private IGenericRepository<Privilege> _privileges;
        private IGenericRepository<RolePrivileges> _rolePrivileges;
        private IGenericRepository<Personnel> _personnel;
        private IGenericRepository<EquipmentAllocation> _equipmentAllocations;
        private IGenericRepository<Room> _rooms;
        private IGenericRepository<Status> _statuses;
        private IGenericRepository<Label> _labels;
        private IGenericRepository<RoomLabel> _roomLabels;
        private IGenericRepository<PersonnelPosition> _personnelPosition;
        private IGenericRepository<LibraryFolder> _libraryFolders;
        private IGenericRepository<LibraryItem> _libraryItems;
        private IGenericRepository<ReportTemplate> _reportTemplates;
        private IGenericRepository<Report> _reports;
        private IGenericRepository<TemsJWT> _jwtBlacklist;
        private IGenericRepository<CommonNotification> _commonNotifications;
        private IGenericRepository<UserNotification> _userNotifications;
        private IGenericRepository<BugReport> _bugReports;
        private IGenericRepository<UserWithBlacklistedToken> _userWithBlacklistedTokens;

        public IGenericRepository<Announcement> Announcements 
            => _announcements ??= new GenericRepository<Announcement>(_context);
        public IGenericRepository<FrequentTicketProblem> FrequentTicketProblems 
            => _frequentTicketProblems ??= new GenericRepository<FrequentTicketProblem>(_context);
        public IGenericRepository<Log> Logs
            => _logs ??= new GenericRepository<Log>(_context);
        public IGenericRepository<Ticket> Tickets
            => _tickets ??= new GenericRepository<Ticket>(_context);
        public IGenericRepository<Equipment> Equipments
            => _equipments ??= new GenericRepository<Equipment>(_context);
        public IGenericRepository<EquipmentDefinition> EquipmentDefinitions
            => _equipmentDefinitions ??= new GenericRepository<EquipmentDefinition>(_context);
        public IGenericRepository<EquipmentSpecifications> EquipmentSpecifications
            => _equipmentSpecifications ??= new GenericRepository<EquipmentSpecifications>(_context);
        public IGenericRepository<EquipmentType> EquipmentTypes
            => _equipmentTypes ??= new GenericRepository<EquipmentType>(_context);
        public IGenericRepository<Property> Properties
            => _properties ??= new GenericRepository<Property>(_context);
        public IGenericRepository<DataType> DataTypes
            => _dataTypes ??= new GenericRepository<DataType>(_context);
        public IGenericRepository<PropertyEquipmentTypeAssociation> PropertyEquipmentTypeAssociations
            => _propertyEquipmentTypeAssociations ??= new GenericRepository<PropertyEquipmentTypeAssociation>(_context);
        public IGenericRepository<Key> Keys
            => _keys ??= new GenericRepository<Key>(_context);
        public IGenericRepository<KeyAllocation> KeyAllocations
            => _keyAllocations ??= new GenericRepository<KeyAllocation>(_context);
        public IGenericRepository<TEMSUser> TEMSUsers
            => _temsUsers ??= new GenericRepository<TEMSUser>(_context);
        public IGenericRepository<IdentityRole> Roles
            => _roles ??= new GenericRepository<IdentityRole>(_context);
        public IGenericRepository<Privilege> Privileges
            => _privileges ??= new GenericRepository<Privilege>(_context);
        public IGenericRepository<RolePrivileges> RolePrivileges
            => _rolePrivileges ??= new GenericRepository<RolePrivileges>(_context);
        public IGenericRepository<Personnel> Personnel
            => _personnel ??= new GenericRepository<Personnel>(_context);
        public IGenericRepository<EquipmentAllocation> EquipmentAllocations
            => _equipmentAllocations ??= new GenericRepository<EquipmentAllocation>(_context);
        public IGenericRepository<Room> Rooms
            => _rooms ??= new GenericRepository<Room>(_context);
        public IGenericRepository<Status> Statuses
            => _statuses ??= new GenericRepository<Status>(_context);
        public IGenericRepository<Label> Labels
            => _labels ??= new GenericRepository<Label>(_context);
        public IGenericRepository<RoomLabel> RoomLabels
            => _roomLabels ??= new GenericRepository<RoomLabel>(_context);
        public IGenericRepository<PersonnelPosition> PersonnelPositions
            => _personnelPosition ??= new GenericRepository<PersonnelPosition>(_context);
        public IGenericRepository<LibraryFolder> LibraryFolders
            => _libraryFolders ??= new GenericRepository<LibraryFolder>(_context);
        public IGenericRepository<LibraryItem> LibraryItems
            => _libraryItems ??= new GenericRepository<LibraryItem>(_context);
        public IGenericRepository<ReportTemplate> ReportTemplates
            => _reportTemplates ??= new GenericRepository<ReportTemplate>(_context);
        public IGenericRepository<Report> Reports
            => _reports ??= new GenericRepository<Report>(_context);
        public IGenericRepository<TemsJWT> JWTBlacklist
            => _jwtBlacklist ??= new GenericRepository<TemsJWT>(_context);
        public IGenericRepository<CommonNotification> CommonNotifications
            => _commonNotifications ??= new GenericRepository<CommonNotification>(_context);
        public IGenericRepository<UserNotification> UserNotifications
            => _userNotifications ??= new GenericRepository<UserNotification>(_context);
        public IGenericRepository<BugReport> BugReports
            => _bugReports ??= new GenericRepository<BugReport>(_context);

        public IGenericRepository<UserWithBlacklistedToken> UserWithBlacklistedTokens
            => _userWithBlacklistedTokens ??= new GenericRepository<UserWithBlacklistedToken>(_context);

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool dispose)
        {
            if (dispose)
                _context.Dispose();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
