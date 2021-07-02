using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class dbInitialization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommonNotifications",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    SendEmail = table.Column<bool>(type: "bit", nullable: false),
                    SendSMS = table.Column<bool>(type: "bit", nullable: false),
                    SendPush = table.Column<bool>(type: "bit", nullable: false),
                    SendBrowser = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataTypes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FrequentTicketProblems",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Problem = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrequentTicketProblems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JWTBlacklist",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JWTBlacklist", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Labels",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LibraryFolders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LibraryFolders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Privileges",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Privileges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePrivileges",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RoleID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PrivilegeID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePrivileges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePrivileges_AspNetRoles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePrivileges_Privileges_PrivilegeID",
                        column: x => x.PrivilegeID,
                        principalTable: "Privileges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Announcements",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    AuthorID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "EquipmentAllocations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EquipmentID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DateAllocated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateArchieved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false),
                    PersonnelID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    RoomID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DateReturned = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArchievedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentAllocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentDefinitions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    EquipmentTypeID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ParentID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ArchievedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateArchieved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentDefinitions_EquipmentDefinitions_ParentID",
                        column: x => x.ParentID,
                        principalTable: "EquipmentDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ParentID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    TEMSID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Price = table.Column<double>(type: "float", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: true),
                    EquipmentDefinitionID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    RegisteredByID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ArchievedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDefect = table.Column<bool>(type: "bit", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    DateArchieved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipments_EquipmentDefinitions_EquipmentDefinitionID",
                        column: x => x.EquipmentDefinitionID,
                        principalTable: "EquipmentDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Equipments_Equipments_ParentID",
                        column: x => x.ParentID,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentSpecifications",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EquipmentDefinitionID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PropertyID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DateArchieved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: true),
                    ArchievedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentSpecifications_EquipmentDefinitions_EquipmentDefinitionID",
                        column: x => x.EquipmentDefinitionID,
                        principalTable: "EquipmentDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentTypes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateArchieved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false),
                    EditableTypeInfo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ArchievedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentTypeEquipmentType",
                columns: table => new
                {
                    ChildrenId = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    ParentsId = table.Column<string>(type: "nvarchar(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentTypeEquipmentType", x => new { x.ChildrenId, x.ParentsId });
                    table.ForeignKey(
                        name: "FK_EquipmentTypeEquipmentType_EquipmentTypes_ChildrenId",
                        column: x => x.ChildrenId,
                        principalTable: "EquipmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentTypeEquipmentType_EquipmentTypes_ParentsId",
                        column: x => x.ParentsId,
                        principalTable: "EquipmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KeyAllocations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PersonnelID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    KeyID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DateArchieved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false),
                    DateAllocated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateReturned = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArchievedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyAllocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Keys",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DateArchieved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RoomId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ArchievedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LibraryItems",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ActualName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DbPath = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FileSize = table.Column<double>(type: "float", nullable: false),
                    Downloads = table.Column<int>(type: "int", nullable: false),
                    DateUploaded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LibraryFolderId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UploadedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LibraryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LibraryItems_LibraryFolders_LibraryFolderId",
                        column: x => x.LibraryFolderId,
                        principalTable: "LibraryFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EquipmentID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    RoomID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PersonnelID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ArchievedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false),
                    DateArchieved = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logs_Equipments_EquipmentID",
                        column: x => x.EquipmentID,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Personnel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ArchievedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateArchieved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personnel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateRegistered = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateArchieved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GetEmailNotifications = table.Column<bool>(type: "bit", nullable: true),
                    PersonnelId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ArchievedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_AspNetUsers_ArchievedById",
                        column: x => x.ArchievedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Personnel_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelPositions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DateArchieved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false),
                    ArchievedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelPositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelPositions_AspNetUsers_ArchievedById",
                        column: x => x.ArchievedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Required = table.Column<bool>(type: "bit", nullable: false),
                    DateArchieved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false),
                    EditablePropertyInfo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Min = table.Column<int>(type: "int", nullable: true),
                    Max = table.Column<int>(type: "int", nullable: true),
                    Options = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DataTypeID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ArchievedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Properties_AspNetUsers_ArchievedById",
                        column: x => x.ArchievedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Properties_DataTypes_DataTypeID",
                        column: x => x.DataTypeID,
                        principalTable: "DataTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Template = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DateGenerated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DBPath = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    GeneratedByID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_AspNetUsers_GeneratedByID",
                        column: x => x.GeneratedByID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReportTemplates",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SeparateBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Header = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Footer = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateArchieved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false),
                    CommonProperties = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ArchievedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportTemplates_AspNetUsers_ArchievedById",
                        column: x => x.ArchievedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportTemplates_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoomLabels",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DateArchieved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false),
                    ArchievedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomLabels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomLabels_AspNetUsers_ArchievedById",
                        column: x => x.ArchievedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Floor = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: true),
                    ArchievedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateArchieved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_AspNetUsers_ArchievedById",
                        column: x => x.ArchievedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ImportanceIndex = table.Column<int>(type: "int", nullable: false),
                    DateArchieved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false),
                    ArchievedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Statuses_AspNetUsers_ArchievedById",
                        column: x => x.ArchievedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserCommonNotification",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NotificationId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Seen = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCommonNotification", x => new { x.UserId, x.NotificationId });
                    table.ForeignKey(
                        name: "FK_UserCommonNotification_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCommonNotification_CommonNotifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "CommonNotifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserNotifications",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    SendEmail = table.Column<bool>(type: "bit", nullable: false),
                    SendSMS = table.Column<bool>(type: "bit", nullable: false),
                    SendPush = table.Column<bool>(type: "bit", nullable: false),
                    SendBrowser = table.Column<bool>(type: "bit", nullable: false),
                    Seen = table.Column<bool>(type: "bit", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotifications_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelPersonnelPosition",
                columns: table => new
                {
                    PersonnelId = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    PositionsId = table.Column<string>(type: "nvarchar(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelPersonnelPosition", x => new { x.PersonnelId, x.PositionsId });
                    table.ForeignKey(
                        name: "FK_PersonnelPersonnelPosition_Personnel_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonnelPersonnelPosition_PersonnelPositions_PositionsId",
                        column: x => x.PositionsId,
                        principalTable: "PersonnelPositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentTypeProperty",
                columns: table => new
                {
                    EquipmentTypesId = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    PropertiesId = table.Column<string>(type: "nvarchar(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentTypeProperty", x => new { x.EquipmentTypesId, x.PropertiesId });
                    table.ForeignKey(
                        name: "FK_EquipmentTypeProperty_EquipmentTypes_EquipmentTypesId",
                        column: x => x.EquipmentTypesId,
                        principalTable: "EquipmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentTypeProperty_Properties_PropertiesId",
                        column: x => x.PropertiesId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyEquipmentTypeAssociations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TypeID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PropertyID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyEquipmentTypeAssociations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyEquipmentTypeAssociations_EquipmentTypes_TypeID",
                        column: x => x.TypeID,
                        principalTable: "EquipmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyEquipmentTypeAssociations_Properties_PropertyID",
                        column: x => x.PropertyID,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentDefinitionReportTemplate",
                columns: table => new
                {
                    EquipmentDefinitionsId = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    ReportTemplatesMemberOfId = table.Column<string>(type: "nvarchar(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentDefinitionReportTemplate", x => new { x.EquipmentDefinitionsId, x.ReportTemplatesMemberOfId });
                    table.ForeignKey(
                        name: "FK_EquipmentDefinitionReportTemplate_EquipmentDefinitions_EquipmentDefinitionsId",
                        column: x => x.EquipmentDefinitionsId,
                        principalTable: "EquipmentDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentDefinitionReportTemplate_ReportTemplates_ReportTemplatesMemberOfId",
                        column: x => x.ReportTemplatesMemberOfId,
                        principalTable: "ReportTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentTypeReportTemplate",
                columns: table => new
                {
                    EquipmentTypesId = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    ReportTemplatesMemberOfId = table.Column<string>(type: "nvarchar(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentTypeReportTemplate", x => new { x.EquipmentTypesId, x.ReportTemplatesMemberOfId });
                    table.ForeignKey(
                        name: "FK_EquipmentTypeReportTemplate_EquipmentTypes_EquipmentTypesId",
                        column: x => x.EquipmentTypesId,
                        principalTable: "EquipmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentTypeReportTemplate_ReportTemplates_ReportTemplatesMemberOfId",
                        column: x => x.ReportTemplatesMemberOfId,
                        principalTable: "ReportTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelReportTemplate",
                columns: table => new
                {
                    ReportTemplatesAssignedId = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    SignatoriesId = table.Column<string>(type: "nvarchar(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelReportTemplate", x => new { x.ReportTemplatesAssignedId, x.SignatoriesId });
                    table.ForeignKey(
                        name: "FK_PersonnelReportTemplate_Personnel_SignatoriesId",
                        column: x => x.SignatoriesId,
                        principalTable: "Personnel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonnelReportTemplate_ReportTemplates_ReportTemplatesAssignedId",
                        column: x => x.ReportTemplatesAssignedId,
                        principalTable: "ReportTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelReportTemplate1",
                columns: table => new
                {
                    PersonnelId = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    ReportTemplatesMemberId = table.Column<string>(type: "nvarchar(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelReportTemplate1", x => new { x.PersonnelId, x.ReportTemplatesMemberId });
                    table.ForeignKey(
                        name: "FK_PersonnelReportTemplate1_Personnel_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonnelReportTemplate1_ReportTemplates_ReportTemplatesMemberId",
                        column: x => x.ReportTemplatesMemberId,
                        principalTable: "ReportTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyReportTemplate",
                columns: table => new
                {
                    PropertiesId = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    ReportTemplatesMemberOfId = table.Column<string>(type: "nvarchar(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyReportTemplate", x => new { x.PropertiesId, x.ReportTemplatesMemberOfId });
                    table.ForeignKey(
                        name: "FK_PropertyReportTemplate_Properties_PropertiesId",
                        column: x => x.PropertiesId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropertyReportTemplate_ReportTemplates_ReportTemplatesMemberOfId",
                        column: x => x.ReportTemplatesMemberOfId,
                        principalTable: "ReportTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelRoom",
                columns: table => new
                {
                    RoomsSupervisoriedId = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    SupervisoriesId = table.Column<string>(type: "nvarchar(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelRoom", x => new { x.RoomsSupervisoriedId, x.SupervisoriesId });
                    table.ForeignKey(
                        name: "FK_PersonnelRoom_Personnel_SupervisoriesId",
                        column: x => x.SupervisoriesId,
                        principalTable: "Personnel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonnelRoom_Rooms_RoomsSupervisoriedId",
                        column: x => x.RoomsSupervisoriedId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportTemplateRoom",
                columns: table => new
                {
                    ReportTemplatesMemberOfId = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    RoomsId = table.Column<string>(type: "nvarchar(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTemplateRoom", x => new { x.ReportTemplatesMemberOfId, x.RoomsId });
                    table.ForeignKey(
                        name: "FK_ReportTemplateRoom_ReportTemplates_ReportTemplatesMemberOfId",
                        column: x => x.ReportTemplatesMemberOfId,
                        principalTable: "ReportTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportTemplateRoom_Rooms_RoomsId",
                        column: x => x.RoomsId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomRoomLabel",
                columns: table => new
                {
                    LabelsId = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    RoomsId = table.Column<string>(type: "nvarchar(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomRoomLabel", x => new { x.LabelsId, x.RoomsId });
                    table.ForeignKey(
                        name: "FK_RoomRoomLabel_RoomLabels_LabelsId",
                        column: x => x.LabelsId,
                        principalTable: "RoomLabels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomRoomLabel_Rooms_RoomsId",
                        column: x => x.RoomsId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsPinned = table.Column<bool>(type: "bit", nullable: false),
                    TrackingNumber = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateClosed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatusId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    LabelId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ClosedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Problem = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ArchievedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DatePinned = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false),
                    DateArchieved = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_AspNetUsers_ArchievedById",
                        column: x => x.ArchievedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_AspNetUsers_ClosedById",
                        column: x => x.ClosedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_Labels_LabelId",
                        column: x => x.LabelId,
                        principalTable: "Labels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentTicket",
                columns: table => new
                {
                    EquipmentsId = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    TicketsId = table.Column<string>(type: "nvarchar(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentTicket", x => new { x.EquipmentsId, x.TicketsId });
                    table.ForeignKey(
                        name: "FK_EquipmentTicket_Equipments_EquipmentsId",
                        column: x => x.EquipmentsId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentTicket_Tickets_TicketsId",
                        column: x => x.TicketsId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelTicket",
                columns: table => new
                {
                    PersonnelId = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    TicketsId = table.Column<string>(type: "nvarchar(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelTicket", x => new { x.PersonnelId, x.TicketsId });
                    table.ForeignKey(
                        name: "FK_PersonnelTicket_Personnel_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonnelTicket_Tickets_TicketsId",
                        column: x => x.TicketsId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomTicket",
                columns: table => new
                {
                    RoomsId = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    TicketsId = table.Column<string>(type: "nvarchar(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomTicket", x => new { x.RoomsId, x.TicketsId });
                    table.ForeignKey(
                        name: "FK_RoomTicket_Rooms_RoomsId",
                        column: x => x.RoomsId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomTicket_Tickets_TicketsId",
                        column: x => x.TicketsId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TEMSUserTicket",
                columns: table => new
                {
                    AssignedTicketsId = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    AssigneesId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TEMSUserTicket", x => new { x.AssignedTicketsId, x.AssigneesId });
                    table.ForeignKey(
                        name: "FK_TEMSUserTicket_AspNetUsers_AssigneesId",
                        column: x => x.AssigneesId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TEMSUserTicket_Tickets_AssignedTicketsId",
                        column: x => x.AssignedTicketsId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TEMSUserTicket1",
                columns: table => new
                {
                    ClosedAndThenReopenedTicketsId = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    PreviouslyClosedById = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TEMSUserTicket1", x => new { x.ClosedAndThenReopenedTicketsId, x.PreviouslyClosedById });
                    table.ForeignKey(
                        name: "FK_TEMSUserTicket1_AspNetUsers_PreviouslyClosedById",
                        column: x => x.PreviouslyClosedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TEMSUserTicket1_Tickets_ClosedAndThenReopenedTicketsId",
                        column: x => x.ClosedAndThenReopenedTicketsId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_AuthorID",
                table: "Announcements",
                column: "AuthorID");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ArchievedById",
                table: "AspNetUsers",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PersonnelId",
                table: "AspNetUsers",
                column: "PersonnelId",
                unique: true,
                filter: "[PersonnelId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAllocations_ArchievedById",
                table: "EquipmentAllocations",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAllocations_EquipmentID",
                table: "EquipmentAllocations",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAllocations_PersonnelID",
                table: "EquipmentAllocations",
                column: "PersonnelID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAllocations_RoomID",
                table: "EquipmentAllocations",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentDefinitionReportTemplate_ReportTemplatesMemberOfId",
                table: "EquipmentDefinitionReportTemplate",
                column: "ReportTemplatesMemberOfId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentDefinitions_ArchievedById",
                table: "EquipmentDefinitions",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentDefinitions_EquipmentTypeID",
                table: "EquipmentDefinitions",
                column: "EquipmentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentDefinitions_Identifier",
                table: "EquipmentDefinitions",
                column: "Identifier");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentDefinitions_ParentID",
                table: "EquipmentDefinitions",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_ArchievedById",
                table: "Equipments",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_EquipmentDefinitionID",
                table: "Equipments",
                column: "EquipmentDefinitionID");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_ParentID",
                table: "Equipments",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_RegisteredByID",
                table: "Equipments",
                column: "RegisteredByID");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_SerialNumber",
                table: "Equipments",
                column: "SerialNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_TEMSID",
                table: "Equipments",
                column: "TEMSID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSpecifications_ArchievedById",
                table: "EquipmentSpecifications",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSpecifications_EquipmentDefinitionID",
                table: "EquipmentSpecifications",
                column: "EquipmentDefinitionID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSpecifications_PropertyID",
                table: "EquipmentSpecifications",
                column: "PropertyID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTicket_TicketsId",
                table: "EquipmentTicket",
                column: "TicketsId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTypeEquipmentType_ParentsId",
                table: "EquipmentTypeEquipmentType",
                column: "ParentsId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTypeProperty_PropertiesId",
                table: "EquipmentTypeProperty",
                column: "PropertiesId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTypeReportTemplate_ReportTemplatesMemberOfId",
                table: "EquipmentTypeReportTemplate",
                column: "ReportTemplatesMemberOfId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTypes_ArchievedById",
                table: "EquipmentTypes",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_KeyAllocations_ArchievedById",
                table: "KeyAllocations",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_KeyAllocations_KeyID",
                table: "KeyAllocations",
                column: "KeyID");

            migrationBuilder.CreateIndex(
                name: "IX_KeyAllocations_PersonnelID",
                table: "KeyAllocations",
                column: "PersonnelID");

            migrationBuilder.CreateIndex(
                name: "IX_Keys_ArchievedById",
                table: "Keys",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_Keys_RoomId",
                table: "Keys",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryItems_LibraryFolderId",
                table: "LibraryItems",
                column: "LibraryFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryItems_UploadedById",
                table: "LibraryItems",
                column: "UploadedById");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ArchievedById",
                table: "Logs",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_CreatedByID",
                table: "Logs",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_EquipmentID",
                table: "Logs",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_PersonnelID",
                table: "Logs",
                column: "PersonnelID");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_RoomID",
                table: "Logs",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_Personnel_ArchievedById",
                table: "Personnel",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelPersonnelPosition_PositionsId",
                table: "PersonnelPersonnelPosition",
                column: "PositionsId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelPositions_ArchievedById",
                table: "PersonnelPositions",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelReportTemplate_SignatoriesId",
                table: "PersonnelReportTemplate",
                column: "SignatoriesId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelReportTemplate1_ReportTemplatesMemberId",
                table: "PersonnelReportTemplate1",
                column: "ReportTemplatesMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelRoom_SupervisoriesId",
                table: "PersonnelRoom",
                column: "SupervisoriesId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelTicket_TicketsId",
                table: "PersonnelTicket",
                column: "TicketsId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_ArchievedById",
                table: "Properties",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_DataTypeID",
                table: "Properties",
                column: "DataTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyEquipmentTypeAssociations_PropertyID",
                table: "PropertyEquipmentTypeAssociations",
                column: "PropertyID");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyEquipmentTypeAssociations_TypeID",
                table: "PropertyEquipmentTypeAssociations",
                column: "TypeID");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyReportTemplate_ReportTemplatesMemberOfId",
                table: "PropertyReportTemplate",
                column: "ReportTemplatesMemberOfId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_GeneratedByID",
                table: "Reports",
                column: "GeneratedByID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplateRoom_RoomsId",
                table: "ReportTemplateRoom",
                column: "RoomsId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplates_ArchievedById",
                table: "ReportTemplates",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplates_CreatedById",
                table: "ReportTemplates",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RolePrivileges_PrivilegeID",
                table: "RolePrivileges",
                column: "PrivilegeID");

            migrationBuilder.CreateIndex(
                name: "IX_RolePrivileges_RoleID",
                table: "RolePrivileges",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_RoomLabels_ArchievedById",
                table: "RoomLabels",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_RoomRoomLabel_RoomsId",
                table: "RoomRoomLabel",
                column: "RoomsId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_ArchievedById",
                table: "Rooms",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_RoomTicket_TicketsId",
                table: "RoomTicket",
                column: "TicketsId");

            migrationBuilder.CreateIndex(
                name: "IX_Statuses_ArchievedById",
                table: "Statuses",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_TEMSUserTicket_AssigneesId",
                table: "TEMSUserTicket",
                column: "AssigneesId");

            migrationBuilder.CreateIndex(
                name: "IX_TEMSUserTicket1_PreviouslyClosedById",
                table: "TEMSUserTicket1",
                column: "PreviouslyClosedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ArchievedById",
                table: "Tickets",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ClosedById",
                table: "Tickets",
                column: "ClosedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CreatedById",
                table: "Tickets",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_LabelId",
                table: "Tickets",
                column: "LabelId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_StatusId",
                table: "Tickets",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCommonNotification_NotificationId",
                table: "UserCommonNotification",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_UserID",
                table: "UserNotifications",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_AspNetUsers_AuthorID",
                table: "Announcements",
                column: "AuthorID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAllocations_AspNetUsers_ArchievedById",
                table: "EquipmentAllocations",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAllocations_Equipments_EquipmentID",
                table: "EquipmentAllocations",
                column: "EquipmentID",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAllocations_Personnel_PersonnelID",
                table: "EquipmentAllocations",
                column: "PersonnelID",
                principalTable: "Personnel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAllocations_Rooms_RoomID",
                table: "EquipmentAllocations",
                column: "RoomID",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentDefinitions_AspNetUsers_ArchievedById",
                table: "EquipmentDefinitions",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentDefinitions_EquipmentTypes_EquipmentTypeID",
                table: "EquipmentDefinitions",
                column: "EquipmentTypeID",
                principalTable: "EquipmentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_AspNetUsers_ArchievedById",
                table: "Equipments",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_AspNetUsers_RegisteredByID",
                table: "Equipments",
                column: "RegisteredByID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentSpecifications_AspNetUsers_ArchievedById",
                table: "EquipmentSpecifications",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentSpecifications_Properties_PropertyID",
                table: "EquipmentSpecifications",
                column: "PropertyID",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentTypes_AspNetUsers_ArchievedById",
                table: "EquipmentTypes",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KeyAllocations_AspNetUsers_ArchievedById",
                table: "KeyAllocations",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KeyAllocations_Keys_KeyID",
                table: "KeyAllocations",
                column: "KeyID",
                principalTable: "Keys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KeyAllocations_Personnel_PersonnelID",
                table: "KeyAllocations",
                column: "PersonnelID",
                principalTable: "Personnel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Keys_AspNetUsers_ArchievedById",
                table: "Keys",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Keys_Rooms_RoomId",
                table: "Keys",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LibraryItems_AspNetUsers_UploadedById",
                table: "LibraryItems",
                column: "UploadedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_AspNetUsers_ArchievedById",
                table: "Logs",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_AspNetUsers_CreatedByID",
                table: "Logs",
                column: "CreatedByID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Personnel_PersonnelID",
                table: "Logs",
                column: "PersonnelID",
                principalTable: "Personnel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Rooms_RoomID",
                table: "Logs",
                column: "RoomID",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Personnel_AspNetUsers_ArchievedById",
                table: "Personnel",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personnel_AspNetUsers_ArchievedById",
                table: "Personnel");

            migrationBuilder.DropTable(
                name: "Announcements");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "EquipmentAllocations");

            migrationBuilder.DropTable(
                name: "EquipmentDefinitionReportTemplate");

            migrationBuilder.DropTable(
                name: "EquipmentSpecifications");

            migrationBuilder.DropTable(
                name: "EquipmentTicket");

            migrationBuilder.DropTable(
                name: "EquipmentTypeEquipmentType");

            migrationBuilder.DropTable(
                name: "EquipmentTypeProperty");

            migrationBuilder.DropTable(
                name: "EquipmentTypeReportTemplate");

            migrationBuilder.DropTable(
                name: "FrequentTicketProblems");

            migrationBuilder.DropTable(
                name: "JWTBlacklist");

            migrationBuilder.DropTable(
                name: "KeyAllocations");

            migrationBuilder.DropTable(
                name: "LibraryItems");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "PersonnelPersonnelPosition");

            migrationBuilder.DropTable(
                name: "PersonnelReportTemplate");

            migrationBuilder.DropTable(
                name: "PersonnelReportTemplate1");

            migrationBuilder.DropTable(
                name: "PersonnelRoom");

            migrationBuilder.DropTable(
                name: "PersonnelTicket");

            migrationBuilder.DropTable(
                name: "PropertyEquipmentTypeAssociations");

            migrationBuilder.DropTable(
                name: "PropertyReportTemplate");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "ReportTemplateRoom");

            migrationBuilder.DropTable(
                name: "RolePrivileges");

            migrationBuilder.DropTable(
                name: "RoomRoomLabel");

            migrationBuilder.DropTable(
                name: "RoomTicket");

            migrationBuilder.DropTable(
                name: "TEMSUserTicket");

            migrationBuilder.DropTable(
                name: "TEMSUserTicket1");

            migrationBuilder.DropTable(
                name: "UserCommonNotification");

            migrationBuilder.DropTable(
                name: "UserNotifications");

            migrationBuilder.DropTable(
                name: "Keys");

            migrationBuilder.DropTable(
                name: "LibraryFolders");

            migrationBuilder.DropTable(
                name: "Equipments");

            migrationBuilder.DropTable(
                name: "PersonnelPositions");

            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.DropTable(
                name: "ReportTemplates");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Privileges");

            migrationBuilder.DropTable(
                name: "RoomLabels");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "CommonNotifications");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "EquipmentDefinitions");

            migrationBuilder.DropTable(
                name: "DataTypes");

            migrationBuilder.DropTable(
                name: "Labels");

            migrationBuilder.DropTable(
                name: "Statuses");

            migrationBuilder.DropTable(
                name: "EquipmentTypes");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Personnel");
        }
    }
}
