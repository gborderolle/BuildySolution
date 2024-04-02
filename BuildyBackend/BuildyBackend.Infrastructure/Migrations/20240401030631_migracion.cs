﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BuildyBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class migracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Creation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Update = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Creation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Update = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                });

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Update = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NominatimCountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Entity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Creation = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Owner",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Update = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owner", x => x.Id);
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
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Province",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Update = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NominatimProvinceCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Province", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Province_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Update = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProvinceId = table.Column<int>(type: "int", nullable: false),
                    NominatimCityCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City", x => x.Id);
                    table.ForeignKey(
                        name: "FK_City_Province_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Province",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Estate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Update = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatLong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GoogleMapsURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstateIsRented = table.Column<bool>(type: "bit", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    PresentRentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estate_City_CityId",
                        column: x => x.CityId,
                        principalTable: "City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Estate_Owner_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Job",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Month = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Update = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LabourCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EstateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Job_Estate_EstateId",
                        column: x => x.EstateId,
                        principalTable: "Estate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Update = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Warrant = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MonthlyValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Datetime_monthInit = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RentIsEnded = table.Column<bool>(type: "bit", nullable: false),
                    EstateId = table.Column<int>(type: "int", nullable: false),
                    PrimaryTenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rent_Estate_EstateId",
                        column: x => x.EstateId,
                        principalTable: "Estate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Month = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Update = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Report_Estate_EstateId",
                        column: x => x.EstateId,
                        principalTable: "Estate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Worker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Update = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdentityDocument = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worker", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Worker_Job_JobId",
                        column: x => x.JobId,
                        principalTable: "Job",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "File",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Update = table.Column<DateTime>(type: "datetime2", nullable: false),
                    URL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.Id);
                    table.ForeignKey(
                        name: "FK_File_Rent_RentId",
                        column: x => x.RentId,
                        principalTable: "Rent",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Update = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Phone1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdentityDocument = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tenant_Rent_RentId",
                        column: x => x.RentId,
                        principalTable: "Rent",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Photo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Update = table.Column<DateTime>(type: "datetime2", nullable: false),
                    URL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportId = table.Column<int>(type: "int", nullable: true),
                    JobId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photo_Job_JobId",
                        column: x => x.JobId,
                        principalTable: "Job",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Photo_Report_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Report",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Creation", "Discriminator", "Name", "NormalizedName", "Update" },
                values: new object[,]
                {
                    { "bef4cbd4-1f2b-472f-a1e2-e1a901f6808c", null, new DateTime(2024, 4, 1, 0, 6, 27, 69, DateTimeKind.Local).AddTicks(3824), "BuildyRole", "Admin", "ADMIN", new DateTime(2024, 4, 1, 0, 6, 27, 69, DateTimeKind.Local).AddTicks(3827) },
                    { "bef4cbd4-1f2b-472f-a3f2-e1a901f6811c", null, new DateTime(2024, 4, 1, 0, 6, 27, 69, DateTimeKind.Local).AddTicks(3848), "BuildyRole", "User", "USER", new DateTime(2024, 4, 1, 0, 6, 27, 69, DateTimeKind.Local).AddTicks(3850) }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Creation", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "Update", "UserName" },
                values: new object[,]
                {
                    { "11c767dc-e8ce-448e-8fdb-ee590a44a3ff", 0, "47b97e1a-e303-4324-8095-bc713244afd2", new DateTime(2024, 4, 1, 0, 6, 27, 537, DateTimeKind.Local).AddTicks(4609), "BuildyUser", "gladys@buildy.lat", false, false, null, "Gladys de los Santos", "GLADYS@BUILDY.LAT", "IRGLA", "AQAAAAIAAYagAAAAEMJISL50U2Cm6RVYBr92VYA5+97zEoo/8xuE5J6Lkmw1oavdSiaNyEaGT6lOocGirQ==", null, false, "a4e048b1-666e-4093-a53c-857979a128e2", false, new DateTime(2024, 4, 1, 0, 6, 27, 537, DateTimeKind.Local).AddTicks(4622), "irgla" },
                    { "58fbedfc-e682-479b-ba46-19ef4c137d2a", 0, "78cdaedf-5e32-49c8-8137-b33d107e9334", new DateTime(2024, 4, 1, 0, 6, 27, 377, DateTimeKind.Local).AddTicks(1840), "BuildyUser", "mirta@buildy.lat", false, false, null, "Mirta de los Santos", "MIRTA@BUILDY.LAT", "MIRTADLS", "AQAAAAIAAYagAAAAEEbg86FvCQmgCmoPx7tnH8Z4MFSsnipu5AKYvbdrvbVGp5/I/0XkjjLHwV/ISW1XSQ==", null, false, "f4315ebe-5aa4-4693-92f8-760fa30bcf1e", false, new DateTime(2024, 4, 1, 0, 6, 27, 377, DateTimeKind.Local).AddTicks(1861), "mirtadls" },
                    { "c2ee6493-5a73-46f3-a3f2-46d1d11d7176", 0, "3e5f083a-2355-4d9b-928a-cfe58d1cef3c", new DateTime(2024, 4, 1, 0, 6, 27, 69, DateTimeKind.Local).AddTicks(4677), "BuildyUser", "admin@buildy.lat", false, false, null, "Usuario administrador", "ADMIN@BUILDY.LAT", "USERADMIN", "AQAAAAIAAYagAAAAEPl1JxEEAeOJ+iY2BX425JDgh0UvWCBqCMKzn3b/kihsREcDPZAGPf1LsLGes3aB0A==", null, false, "67071d7f-0a28-4661-a913-64bc01bf0c0a", false, new DateTime(2024, 4, 1, 0, 6, 27, 69, DateTimeKind.Local).AddTicks(4680), "useradmin" },
                    { "e0765c93-676c-4199-b7ee-d7877c471821", 0, "f0b11bfc-b55c-447c-aada-8bf881df7030", new DateTime(2024, 4, 1, 0, 6, 27, 246, DateTimeKind.Local).AddTicks(8248), "BuildyUser", "normal@buildy.lat", false, false, null, "Usuario normal", "NORMAL@BUILDY.LAT", "USERNORMAL", "AQAAAAIAAYagAAAAEOcsraTXmDSZT+bVrLpJPpI5TL+UQUs550DwubyAz5hLnn7merchCClgms9Ug5IEqw==", null, false, "fb1e4432-a51a-46c4-b690-ee42376f715c", false, new DateTime(2024, 4, 1, 0, 6, 27, 246, DateTimeKind.Local).AddTicks(8265), "usernormal" }
                });

            migrationBuilder.InsertData(
                table: "Country",
                columns: new[] { "Id", "Creation", "Name", "NominatimCountryCode", "Update" },
                values: new object[] { 1, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6579), "Uruguay", "UY", new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6597) });

            migrationBuilder.InsertData(
                table: "Owner",
                columns: new[] { "Id", "Color", "Creation", "Name", "Update" },
                values: new object[,]
                {
                    { 1, "violet", new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6920), "Mirta", new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6921) },
                    { 2, "orange", new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6923), "Gladys", new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6924) },
                    { 3, "green", new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6927), "Cristina", new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6927) }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "UserId" },
                values: new object[,]
                {
                    { 1, "role", "Admin", "c2ee6493-5a73-46f3-a3f2-46d1d11d7176" },
                    { 2, "role", "User", "e0765c93-676c-4199-b7ee-d7877c471821" },
                    { 3, "role", "User", "58fbedfc-e682-479b-ba46-19ef4c137d2a" },
                    { 4, "role", "User", "11c767dc-e8ce-448e-8fdb-ee590a44a3ff" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "bef4cbd4-1f2b-472f-a1e2-e1a901f6808c", "11c767dc-e8ce-448e-8fdb-ee590a44a3ff" },
                    { "bef4cbd4-1f2b-472f-a1e2-e1a901f6808c", "58fbedfc-e682-479b-ba46-19ef4c137d2a" },
                    { "bef4cbd4-1f2b-472f-a1e2-e1a901f6808c", "c2ee6493-5a73-46f3-a3f2-46d1d11d7176" },
                    { "bef4cbd4-1f2b-472f-a1e2-e1a901f6808c", "e0765c93-676c-4199-b7ee-d7877c471821" }
                });

            migrationBuilder.InsertData(
                table: "Province",
                columns: new[] { "Id", "CountryId", "Creation", "Name", "NominatimProvinceCode", "Update" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6755), "Cerro Largo", "CL", new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6756) },
                    { 2, 1, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6758), "Montevideo", "MO", new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6761) }
                });

            migrationBuilder.InsertData(
                table: "City",
                columns: new[] { "Id", "Creation", "Name", "NominatimCityCode", "ProvinceId", "Update" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6838), "Melo", "ME", 1, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6841) },
                    { 2, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6844), "Montevideo", "MO", 2, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6844) }
                });

            migrationBuilder.InsertData(
                table: "Estate",
                columns: new[] { "Id", "Address", "CityId", "Comments", "Creation", "EstateIsRented", "GoogleMapsURL", "LatLong", "Name", "OwnerId", "PresentRentId", "Update" },
                values: new object[,]
                {
                    { 1, "Colón 476", 1, null, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7005), false, "https://www.google.com/maps/place/C.+Col%C3%B3n+476,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3674814,-54.1757085,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980b6fbe707:0x3f99b6b98961385b!8m2!3d-32.3674814!4d-54.1731282!16s%2Fg%2F11fn9p0pws?hl=es-419&entry=ttu", "-32.3674814,-54.1757085", "Colón 476", 3, 0, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7008) },
                    { 2, "Colón 480", 1, null, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7023), false, "https://www.google.com/maps/place/C.+Col%C3%B3n+480,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3674762,-54.1755751,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980b0c5f12b:0xaf5b5ba2ea4eb1cb!8m2!3d-32.3674762!4d-54.1729948!16s%2Fg%2F11rz98nq5_?hl=es-419&entry=ttu", "-32.3674762,-54.1755751", "Colón 480", 2, 0, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7023) },
                    { 3, "Colón 491", 1, null, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7028), false, "https://www.google.com/maps/place/C.+Col%C3%B3n+491,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.367241,-54.1754153,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980b180e29b:0x95542a9eb2345b16!8m2!3d-32.367241!4d-54.172835!16s%2Fg%2F11fctc9z9z?hl=es-419&entry=ttu", "-32.367241,-54.1754153", "Colón 491", 2, 0, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7029) },
                    { 4, "Colón 495", 1, null, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7034), false, "https://www.google.com/maps/place/C.+Col%C3%B3n+495,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3672195,-54.1755857,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980b3f659db:0xf3c82a67cd3ebe76!8m2!3d-32.3672195!4d-54.1730054!16s%2Fg%2F11gmz50cjj?hl=es-419&entry=ttu", "-32.3672195,-54.1755857", "Colón 495", 2, 0, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7034) },
                    { 5, "Colón 503", 1, null, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7039), false, "https://www.google.com/maps/place/C.+Col%C3%B3n+503,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3672809,-54.1751337,17z/data=!3m1!4b1!4m5!3m4!1s0x95092980a659bd3f:0x1f64283da64670a9!8m2!3d-32.3672809!4d-54.1725534?hl=es-419&entry=ttu", "-32.3672809,-54.1751337", "Colón 503", 2, 0, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7040) },
                    { 6, "Darío Silva 774", 1, null, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7042), false, "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+774,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3679807,-54.1752022,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980a2c60edb:0x48ad692931a026ea!8m2!3d-32.3679807!4d-54.1726219!16s%2Fg%2F11gr6dzf3g?hl=es-419&entry=ttu", "-32.3679807,-54.1752022", "Darío Silva 774", 1, 0, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7043) },
                    { 7, "Darío Silva 774 BIS", 1, null, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7047), false, "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+774,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3679807,-54.1752022,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980a2c60edb:0x48ad692931a026ea!8m2!3d-32.3679807!4d-54.1726219!16s%2Fg%2F11gr6dzf3g?hl=es-419&entry=ttu", "-32.3679807,-54.1752022", "Darío Silva 774 BIS", 1, 0, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7048) },
                    { 8, "Darío Silva 781", 1, null, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7051), false, "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+781,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3675191,-54.1753876,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980b076246d:0x1b122d0ac3c1dbc7!8m2!3d-32.3675191!4d-54.1728073!16s%2Fg%2F11g194j1tj?hl=es-419&entry=ttu", "-32.3675191,-54.1753876", "Darío Silva 781", 1, 0, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7052) },
                    { 9, "Darío Silva 785", 1, null, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7055), false, "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+785,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3676402,-54.1754579,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980ba5f73d1:0x186b45d8ed124b2a!8m2!3d-32.3676402!4d-54.1728776!16s%2Fg%2F11sb62lb6c?hl=es-419&entry=ttu", "-32.3676402,-54.1754579", "Darío Silva 785", 2, 0, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7055) },
                    { 10, "Darío Silva 789", 1, null, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7059), false, "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+789,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3676919,-54.1754685,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980ba535f25:0xbdc6468d3ed51cc1!8m2!3d-32.3676919!4d-54.1728882!16s%2Fg%2F11syz1ryh9?hl=es-419&entry=ttu", "-32.3676919,-54.1754685", "Darío Silva 789", 1, 0, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7059) },
                    { 11, "Darío Silva 793", 1, null, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7063), false, "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+793,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3675108,-54.1754001,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980b079be31:0xa19a7a3d822a0595!8m2!3d-32.3675108!4d-54.1728198!16s%2Fg%2F11h_c4rxz9?hl=es-419&entry=ttu", "-32.3675108,-54.1754001", "Darío Silva 793", 1, 0, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7064) },
                    { 12, "Darío Silva 801", 1, null, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7066), false, "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+801,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3673741,-54.1752804,17z/data=!3m1!4b1!4m5!3m4!1s0x95092980ad44ba05:0x3da437edd983606!8m2!3d-32.3673741!4d-54.1727001?hl=es-419&entry=ttu", "-32.3673741,-54.1752804", "Darío Silva 801", 1, 0, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7067) },
                    { 13, "Darío Silva 803", 1, null, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7071), false, "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+803,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3673108,-54.1755231,17z/data=!3m1!4b1!4m5!3m4!1s0x95092980ad44ba05:0x24b5f3fc904ac228!8m2!3d-32.3673108!4d-54.1729428?hl=es-419&entry=ttu", "-32.3673108,-54.1755231", "Darío Silva 803", 1, 0, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7071) },
                    { 14, "Darío Silva Cochera", 1, null, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7074), false, "https://www.google.com/maps/place/C.+Col%C3%B3n+476,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3674814,-54.1757085,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980b6fbe707:0x3f99b6b98961385b!8m2!3d-32.3674814!4d-54.1731282!16s%2Fg%2F11fn9p0pws?hl=es-419&entry=ttu", "-32.3674814,-54.1757085", "Darío Silva Cochera", 1, 0, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7074) },
                    { 15, "Treinta y Tres 299", 1, null, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7078), false, "https://www.google.com/maps/place/Treinta+y+Tres+299,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3765821,-54.1703877,17z/data=!3m1!4b1!4m6!3m5!1s0x95092bd6c1beccc3:0xb91b3165ec6107e3!8m2!3d-32.3765821!4d-54.1678074!16s%2Fg%2F11gmz7h_cl?hl=es-419&entry=ttu", "-32.3765821,-54.1703877", "Treinta y Tres 299", 1, 0, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7078) },
                    { 16, "Manuel Oribe 788", 1, null, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7082), false, "https://www.google.com/maps/place/C.+Gral.+Manuel+Oribe+788,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3764414,-54.1706063,17z/data=!3m1!4b1!4m6!3m5!1s0x95092bd69533354f:0xd6a6a68977288372!8m2!3d-32.3764414!4d-54.168026!16s%2Fg%2F11h57sb583?hl=es-419&entry=ttu", "-32.3764414,-54.1706063", "Manuel Oribe 788", 1, 0, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7084) },
                    { 17, "Rincón Artigas 702", 1, null, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7146), false, "https://www.google.com/maps/place/Dr+Rincon+Artigas+702,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3690096,-54.176412,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980ee49a557:0xfbaefe3d055f9ab9!8m2!3d-32.3690096!4d-54.1738317!16s%2Fg%2F11h_c4zxvw?hl=es-419&entry=ttu", "-32.3690096,-54.176412", "Rincón Artigas 702", 2, 0, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7149) },
                    { 18, "Maldonado 1106", 2, null, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7155), false, "https://www.google.com/maps/place/Maldonado+1106,+11100+Montevideo,+Departamento+de+Montevideo/@-34.9097969,-56.1945273,17z/data=!3m1!4b1!4m6!3m5!1s0x959f81c066a6fb4d:0xdb3d1d7d172a0f4c!8m2!3d-34.9097969!4d-56.191947!16s%2Fg%2F11fhvn7njf?hl=es-419&entry=ttu", "-34.9097969,-56.1945273", "Maldonado 1106", 1, 0, new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7156) }
                });

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
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_City_ProvinceId",
                table: "City",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Estate_CityId",
                table: "Estate",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Estate_OwnerId",
                table: "Estate",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_File_RentId",
                table: "File",
                column: "RentId");

            migrationBuilder.CreateIndex(
                name: "IX_Job_EstateId",
                table: "Job",
                column: "EstateId");

            migrationBuilder.CreateIndex(
                name: "IX_Photo_JobId",
                table: "Photo",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Photo_ReportId",
                table: "Photo",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Province_CountryId",
                table: "Province",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Rent_EstateId",
                table: "Rent",
                column: "EstateId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_EstateId",
                table: "Report",
                column: "EstateId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_RentId",
                table: "Tenant",
                column: "RentId");

            migrationBuilder.CreateIndex(
                name: "IX_Worker_JobId",
                table: "Worker",
                column: "JobId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "File");

            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "Photo");

            migrationBuilder.DropTable(
                name: "Tenant");

            migrationBuilder.DropTable(
                name: "Worker");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropTable(
                name: "Rent");

            migrationBuilder.DropTable(
                name: "Job");

            migrationBuilder.DropTable(
                name: "Estate");

            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropTable(
                name: "Owner");

            migrationBuilder.DropTable(
                name: "Province");

            migrationBuilder.DropTable(
                name: "Country");
        }
    }
}