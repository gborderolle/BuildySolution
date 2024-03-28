using System;
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
                name: "CountryDS",
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
                    table.PrimaryKey("PK_CountryDS", x => x.Id);
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
                name: "OwnerDS",
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
                    table.PrimaryKey("PK_OwnerDS", x => x.Id);
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
                name: "ProvinceDS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Update = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NominatimProvinceCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryDSId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvinceDS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProvinceDS_CountryDS_CountryDSId",
                        column: x => x.CountryDSId,
                        principalTable: "CountryDS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CityDS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Update = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProvinceDSId = table.Column<int>(type: "int", nullable: false),
                    NominatimCityCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CityDS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CityDS_ProvinceDS_ProvinceDSId",
                        column: x => x.ProvinceDSId,
                        principalTable: "ProvinceDS",
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
                    CityDSId = table.Column<int>(type: "int", nullable: false),
                    OwnerDSId = table.Column<int>(type: "int", nullable: false),
                    PresentRentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estate_CityDS_CityDSId",
                        column: x => x.CityDSId,
                        principalTable: "CityDS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Estate_OwnerDS_OwnerDSId",
                        column: x => x.OwnerDSId,
                        principalTable: "OwnerDS",
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
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdentityDocument = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    Phone2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdentityDocument = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    { "bef4cbd4-1f2b-472f-a1e2-e1a901f6808c", null, new DateTime(2024, 3, 28, 19, 5, 38, 532, DateTimeKind.Local).AddTicks(6820), "BuildyRole", "Admin", "ADMIN", new DateTime(2024, 3, 28, 19, 5, 38, 532, DateTimeKind.Local).AddTicks(6821) },
                    { "bef4cbd4-1f2b-472f-a3f2-e1a901f6811c", null, new DateTime(2024, 3, 28, 19, 5, 38, 532, DateTimeKind.Local).AddTicks(6830), "BuildyRole", "User", "USER", new DateTime(2024, 3, 28, 19, 5, 38, 532, DateTimeKind.Local).AddTicks(6831) }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Creation", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "Update", "UserName" },
                values: new object[,]
                {
                    { "11c767dc-e8ce-448e-8fdb-ee590a44a3ff", 0, "7609eac6-08ee-482e-bac0-0bda4d9e5d7f", new DateTime(2024, 3, 28, 19, 5, 38, 814, DateTimeKind.Local).AddTicks(1857), "BuildyUser", "gladys@buildy.lat", false, false, null, "Gladys de los Santos", "GLADYS@BUILDY.LAT", "IRGLA", "AQAAAAIAAYagAAAAELD5VwPo7LutdySw8JQPUxQXjzo+bDpJuTBTD7iVp1V2ovAOpmNNX1hDjgMHDv9uHQ==", null, false, "9552aedb-e574-4672-a3eb-06bb1bbdfa95", false, new DateTime(2024, 3, 28, 19, 5, 38, 814, DateTimeKind.Local).AddTicks(1865), "irgla" },
                    { "58fbedfc-e682-479b-ba46-19ef4c137d2a", 0, "674287bd-180f-4687-916b-adee8a904c90", new DateTime(2024, 3, 28, 19, 5, 38, 719, DateTimeKind.Local).AddTicks(2918), "BuildyUser", "mirta@buildy.lat", false, false, null, "Mirta de los Santos", "MIRTA@BUILDY.LAT", "MIRTADLS", "AQAAAAIAAYagAAAAEMSzRYiIcTJXZBzJDzejhxtnDGRbmyltngmrmmDiOomXV8CkYTTgALgmXF6en7cEOw==", null, false, "1bb2b23d-fa27-4701-8601-9fd5d7338c6f", false, new DateTime(2024, 3, 28, 19, 5, 38, 719, DateTimeKind.Local).AddTicks(2923), "mirtadls" },
                    { "c2ee6493-5a73-46f3-a3f2-46d1d11d7176", 0, "115f0da7-4712-4ca6-8eb1-5c35f70340cc", new DateTime(2024, 3, 28, 19, 5, 38, 532, DateTimeKind.Local).AddTicks(7255), "BuildyUser", "admin@buildy.lat", false, false, null, "Usuario administrador", "ADMIN@BUILDY.LAT", "USERADMIN", "AQAAAAIAAYagAAAAEGrgcqn37aSK9n7E1VKBSkZTIX4uInNXAVW4PNgTCezg5S6YSyKaMPdXc9wq2xKW4A==", null, false, "a7f067a5-7c15-4812-b6bc-91ae43a165be", false, new DateTime(2024, 3, 28, 19, 5, 38, 532, DateTimeKind.Local).AddTicks(7257), "useradmin" },
                    { "e0765c93-676c-4199-b7ee-d7877c471821", 0, "4301e30d-6743-4a09-b360-3f6a86a35522", new DateTime(2024, 3, 28, 19, 5, 38, 621, DateTimeKind.Local).AddTicks(2913), "BuildyUser", "normal@buildy.lat", false, false, null, "Usuario normal", "NORMAL@BUILDY.LAT", "USERNORMAL", "AQAAAAIAAYagAAAAELA0YHSlAaJaCqAlEyxE2WbvyS+97yjq90VUZIyzNH+FhjKNTQmsVTjUQhc541CSWQ==", null, false, "f8829aba-e195-4b75-8bd9-2f24f7df3399", false, new DateTime(2024, 3, 28, 19, 5, 38, 621, DateTimeKind.Local).AddTicks(2927), "usernormal" }
                });

            migrationBuilder.InsertData(
                table: "CountryDS",
                columns: new[] { "Id", "Creation", "Name", "NominatimCountryCode", "Update" },
                values: new object[] { 1, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1044), "Uruguay", "UY", new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1051) });

            migrationBuilder.InsertData(
                table: "OwnerDS",
                columns: new[] { "Id", "Color", "Creation", "Name", "Update" },
                values: new object[,]
                {
                    { 1, "violet", new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1523), "Mirta", new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1524) },
                    { 2, "orange", new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1525), "Gladys", new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1526) },
                    { 3, "green", new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1527), "Cristina", new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1527) }
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
                table: "ProvinceDS",
                columns: new[] { "Id", "CountryDSId", "Creation", "Name", "NominatimProvinceCode", "Update" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1332), "Cerro Largo", "CL", new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1333) },
                    { 2, 1, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1334), "Montevideo", "MO", new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1335) }
                });

            migrationBuilder.InsertData(
                table: "CityDS",
                columns: new[] { "Id", "Creation", "Name", "NominatimCityCode", "ProvinceDSId", "Update" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1437), "Melo", "ME", 1, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1438) },
                    { 2, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1439), "Montevideo", "MO", 2, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1440) }
                });

            migrationBuilder.InsertData(
                table: "Estate",
                columns: new[] { "Id", "Address", "CityDSId", "Comments", "Creation", "EstateIsRented", "GoogleMapsURL", "LatLong", "Name", "OwnerDSId", "PresentRentId", "Update" },
                values: new object[,]
                {
                    { 1, "Colón 476", 1, null, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1607), false, "https://www.google.com/maps/place/C.+Col%C3%B3n+476,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3674814,-54.1757085,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980b6fbe707:0x3f99b6b98961385b!8m2!3d-32.3674814!4d-54.1731282!16s%2Fg%2F11fn9p0pws?hl=es-419&entry=ttu", "-32.3674814,-54.1757085", "Colón 476", 3, 0, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1607) },
                    { 2, "Colón 480", 1, null, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1648), false, "https://www.google.com/maps/place/C.+Col%C3%B3n+480,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3674762,-54.1755751,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980b0c5f12b:0xaf5b5ba2ea4eb1cb!8m2!3d-32.3674762!4d-54.1729948!16s%2Fg%2F11rz98nq5_?hl=es-419&entry=ttu", "-32.3674762,-54.1755751", "Colón 480", 2, 0, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1648) },
                    { 3, "Colón 491", 1, null, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1651), false, "https://www.google.com/maps/place/C.+Col%C3%B3n+491,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.367241,-54.1754153,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980b180e29b:0x95542a9eb2345b16!8m2!3d-32.367241!4d-54.172835!16s%2Fg%2F11fctc9z9z?hl=es-419&entry=ttu", "-32.367241,-54.1754153", "Colón 491", 2, 0, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1651) },
                    { 4, "Colón 495", 1, null, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1653), false, "https://www.google.com/maps/place/C.+Col%C3%B3n+495,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3672195,-54.1755857,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980b3f659db:0xf3c82a67cd3ebe76!8m2!3d-32.3672195!4d-54.1730054!16s%2Fg%2F11gmz50cjj?hl=es-419&entry=ttu", "-32.3672195,-54.1755857", "Colón 495", 2, 0, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1654) },
                    { 5, "Colón 503", 1, null, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1659), false, "https://www.google.com/maps/place/C.+Col%C3%B3n+503,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3672809,-54.1751337,17z/data=!3m1!4b1!4m5!3m4!1s0x95092980a659bd3f:0x1f64283da64670a9!8m2!3d-32.3672809!4d-54.1725534?hl=es-419&entry=ttu", "-32.3672809,-54.1751337", "Colón 503", 2, 0, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1664) },
                    { 6, "Darío Silva 774", 1, null, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1671), false, "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+774,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3679807,-54.1752022,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980a2c60edb:0x48ad692931a026ea!8m2!3d-32.3679807!4d-54.1726219!16s%2Fg%2F11gr6dzf3g?hl=es-419&entry=ttu", "-32.3679807,-54.1752022", "Darío Silva 774", 1, 0, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1672) },
                    { 7, "Darío Silva 774 BIS", 1, null, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1674), false, "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+774,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3679807,-54.1752022,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980a2c60edb:0x48ad692931a026ea!8m2!3d-32.3679807!4d-54.1726219!16s%2Fg%2F11gr6dzf3g?hl=es-419&entry=ttu", "-32.3679807,-54.1752022", "Darío Silva 774 BIS", 1, 0, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1675) },
                    { 8, "Darío Silva 781", 1, null, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1678), false, "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+781,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3675191,-54.1753876,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980b076246d:0x1b122d0ac3c1dbc7!8m2!3d-32.3675191!4d-54.1728073!16s%2Fg%2F11g194j1tj?hl=es-419&entry=ttu", "-32.3675191,-54.1753876", "Darío Silva 781", 1, 0, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1678) },
                    { 9, "Darío Silva 785", 1, null, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1684), false, "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+785,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3676402,-54.1754579,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980ba5f73d1:0x186b45d8ed124b2a!8m2!3d-32.3676402!4d-54.1728776!16s%2Fg%2F11sb62lb6c?hl=es-419&entry=ttu", "-32.3676402,-54.1754579", "Darío Silva 785", 2, 0, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1684) },
                    { 10, "Darío Silva 789", 1, null, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1686), false, "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+789,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3676919,-54.1754685,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980ba535f25:0xbdc6468d3ed51cc1!8m2!3d-32.3676919!4d-54.1728882!16s%2Fg%2F11syz1ryh9?hl=es-419&entry=ttu", "-32.3676919,-54.1754685", "Darío Silva 789", 1, 0, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1687) },
                    { 11, "Darío Silva 793", 1, null, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1688), false, "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+793,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3675108,-54.1754001,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980b079be31:0xa19a7a3d822a0595!8m2!3d-32.3675108!4d-54.1728198!16s%2Fg%2F11h_c4rxz9?hl=es-419&entry=ttu", "-32.3675108,-54.1754001", "Darío Silva 793", 1, 0, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1689) },
                    { 12, "Darío Silva 801", 1, null, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1690), false, "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+801,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3673741,-54.1752804,17z/data=!3m1!4b1!4m5!3m4!1s0x95092980ad44ba05:0x3da437edd983606!8m2!3d-32.3673741!4d-54.1727001?hl=es-419&entry=ttu", "-32.3673741,-54.1752804", "Darío Silva 801", 1, 0, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1691) },
                    { 13, "Darío Silva 803", 1, null, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1856), false, "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+803,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3673108,-54.1755231,17z/data=!3m1!4b1!4m5!3m4!1s0x95092980ad44ba05:0x24b5f3fc904ac228!8m2!3d-32.3673108!4d-54.1729428?hl=es-419&entry=ttu", "-32.3673108,-54.1755231", "Darío Silva 803", 1, 0, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1856) },
                    { 14, "Darío Silva Cochera", 1, null, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1858), false, "https://www.google.com/maps/place/C.+Col%C3%B3n+476,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3674814,-54.1757085,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980b6fbe707:0x3f99b6b98961385b!8m2!3d-32.3674814!4d-54.1731282!16s%2Fg%2F11fn9p0pws?hl=es-419&entry=ttu", "-32.3674814,-54.1757085", "Darío Silva Cochera", 1, 0, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1858) },
                    { 15, "Treinta y Tres 299", 1, null, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1860), false, "https://www.google.com/maps/place/Treinta+y+Tres+299,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3765821,-54.1703877,17z/data=!3m1!4b1!4m6!3m5!1s0x95092bd6c1beccc3:0xb91b3165ec6107e3!8m2!3d-32.3765821!4d-54.1678074!16s%2Fg%2F11gmz7h_cl?hl=es-419&entry=ttu", "-32.3765821,-54.1703877", "Treinta y Tres 299", 1, 0, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1860) },
                    { 16, "Manuel Oribe 788", 1, null, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1862), false, "https://www.google.com/maps/place/C.+Gral.+Manuel+Oribe+788,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3764414,-54.1706063,17z/data=!3m1!4b1!4m6!3m5!1s0x95092bd69533354f:0xd6a6a68977288372!8m2!3d-32.3764414!4d-54.168026!16s%2Fg%2F11h57sb583?hl=es-419&entry=ttu", "-32.3764414,-54.1706063", "Manuel Oribe 788", 1, 0, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1863) },
                    { 17, "Rincón Artigas 702", 1, null, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1864), false, "https://www.google.com/maps/place/Dr+Rincon+Artigas+702,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3690096,-54.176412,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980ee49a557:0xfbaefe3d055f9ab9!8m2!3d-32.3690096!4d-54.1738317!16s%2Fg%2F11h_c4zxvw?hl=es-419&entry=ttu", "-32.3690096,-54.176412", "Rincón Artigas 702", 2, 0, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1865) },
                    { 18, "Maldonado 1106", 2, null, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1866), false, "https://www.google.com/maps/place/Maldonado+1106,+11100+Montevideo,+Departamento+de+Montevideo/@-34.9097969,-56.1945273,17z/data=!3m1!4b1!4m6!3m5!1s0x959f81c066a6fb4d:0xdb3d1d7d172a0f4c!8m2!3d-34.9097969!4d-56.191947!16s%2Fg%2F11fhvn7njf?hl=es-419&entry=ttu", "-34.9097969,-56.1945273", "Maldonado 1106", 1, 0, new DateTime(2024, 3, 28, 19, 5, 38, 916, DateTimeKind.Local).AddTicks(1867) }
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
                name: "IX_CityDS_ProvinceDSId",
                table: "CityDS",
                column: "ProvinceDSId");

            migrationBuilder.CreateIndex(
                name: "IX_Estate_CityDSId",
                table: "Estate",
                column: "CityDSId");

            migrationBuilder.CreateIndex(
                name: "IX_Estate_OwnerDSId",
                table: "Estate",
                column: "OwnerDSId");

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
                name: "IX_ProvinceDS_CountryDSId",
                table: "ProvinceDS",
                column: "CountryDSId");

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
                name: "CityDS");

            migrationBuilder.DropTable(
                name: "OwnerDS");

            migrationBuilder.DropTable(
                name: "ProvinceDS");

            migrationBuilder.DropTable(
                name: "CountryDS");
        }
    }
}
