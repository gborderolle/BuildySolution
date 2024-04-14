using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildyBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class migracion1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bef4cbd4-1f2b-472f-a1e2-e1a901f6808c",
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 14, 830, DateTimeKind.Local).AddTicks(4002), new DateTime(2024, 4, 9, 22, 44, 14, 830, DateTimeKind.Local).AddTicks(4003) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bef4cbd4-1f2b-472f-a3f2-e1a901f6811c",
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 14, 830, DateTimeKind.Local).AddTicks(4014), new DateTime(2024, 4, 9, 22, 44, 14, 830, DateTimeKind.Local).AddTicks(4015) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "11c767dc-e8ce-448e-8fdb-ee590a44a3ff",
                columns: new[] { "ConcurrencyStamp", "Creation", "PasswordHash", "SecurityStamp", "Update" },
                values: new object[] { "472b4c1b-0da2-421f-8337-4d9b1ce1b150", new DateTime(2024, 4, 9, 22, 44, 15, 427, DateTimeKind.Local).AddTicks(1116), "AQAAAAIAAYagAAAAEIERG+saNBvj2SM+A2XwvGs3JfhH6+/Vqxoqx4TPSDNhosYxN4tnZc8aPGysOhoRfg==", "f14322a1-57e4-48bf-ace6-a47ecfdbd51f", new DateTime(2024, 4, 9, 22, 44, 15, 427, DateTimeKind.Local).AddTicks(1124) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "58fbedfc-e682-479b-ba46-19ef4c137d2a",
                columns: new[] { "ConcurrencyStamp", "Creation", "PasswordHash", "SecurityStamp", "Update" },
                values: new object[] { "4fdd62a0-c3bb-4dbf-9c42-d009b157a31d", new DateTime(2024, 4, 9, 22, 44, 15, 239, DateTimeKind.Local).AddTicks(8288), "AQAAAAIAAYagAAAAEM4+brcxtfiST+sGHQk+KQCDSJ3E0kcxRa1XGdYJ0KHWkgKJiilTIDISvMJwUh5JNA==", "b58677c4-56d5-40ad-8516-b470a0a5b1d9", new DateTime(2024, 4, 9, 22, 44, 15, 239, DateTimeKind.Local).AddTicks(8294) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c2ee6493-5a73-46f3-a3f2-46d1d11d7176",
                columns: new[] { "ConcurrencyStamp", "Creation", "PasswordHash", "SecurityStamp", "Update" },
                values: new object[] { "5dcf8a5c-92c4-4eb6-ad05-7fcb1525f376", new DateTime(2024, 4, 9, 22, 44, 14, 830, DateTimeKind.Local).AddTicks(4452), "AQAAAAIAAYagAAAAEF9l2+xD3VCAB4pxtzK7uFKTS8fWd7dF+YDzKdAOkwcXFZnL3E9IgMcZWkpmtzZTGw==", "7072fe04-3859-4ec0-90e9-c65dd9d81884", new DateTime(2024, 4, 9, 22, 44, 14, 830, DateTimeKind.Local).AddTicks(4454) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e0765c93-676c-4199-b7ee-d7877c471821",
                columns: new[] { "ConcurrencyStamp", "Creation", "PasswordHash", "SecurityStamp", "Update" },
                values: new object[] { "fdbd66d4-cfb4-4510-9603-7eefb9b705ea", new DateTime(2024, 4, 9, 22, 44, 15, 22, DateTimeKind.Local).AddTicks(8766), "AQAAAAIAAYagAAAAEMrH5L4JCqkJ8buLiRDrRhLAqA4e5h4jSTUptFpFIZjlwA5Lo4Xe1FQtTNHhdOJL9Q==", "fc57d7b0-bb3c-4cd1-bc84-428c894b01ba", new DateTime(2024, 4, 9, 22, 44, 15, 22, DateTimeKind.Local).AddTicks(8772) });

            migrationBuilder.UpdateData(
                table: "City",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(646), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(648) });

            migrationBuilder.UpdateData(
                table: "City",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(652), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(653) });

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(215), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(224) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(986), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(987) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1033), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1034) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1039), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1039) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1042), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1043) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1056), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1057) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1060), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1061) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1064), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1064) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1071), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1072) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1074), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1075) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1078), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1079) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1082), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1082) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1085), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1086) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1089), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1090) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1093), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1094) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1097), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1098) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1101), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1102) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1105), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1106) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1108), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(1109) });

            migrationBuilder.UpdateData(
                table: "Owner",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(851), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(852) });

            migrationBuilder.UpdateData(
                table: "Owner",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(855), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(870) });

            migrationBuilder.UpdateData(
                table: "Owner",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(872), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(873) });

            migrationBuilder.UpdateData(
                table: "Province",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(524), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(526) });

            migrationBuilder.UpdateData(
                table: "Province",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(530), new DateTime(2024, 4, 9, 22, 44, 15, 615, DateTimeKind.Local).AddTicks(531) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bef4cbd4-1f2b-472f-a1e2-e1a901f6808c",
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 69, DateTimeKind.Local).AddTicks(3824), new DateTime(2024, 4, 1, 0, 6, 27, 69, DateTimeKind.Local).AddTicks(3827) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bef4cbd4-1f2b-472f-a3f2-e1a901f6811c",
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 69, DateTimeKind.Local).AddTicks(3848), new DateTime(2024, 4, 1, 0, 6, 27, 69, DateTimeKind.Local).AddTicks(3850) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "11c767dc-e8ce-448e-8fdb-ee590a44a3ff",
                columns: new[] { "ConcurrencyStamp", "Creation", "PasswordHash", "SecurityStamp", "Update" },
                values: new object[] { "47b97e1a-e303-4324-8095-bc713244afd2", new DateTime(2024, 4, 1, 0, 6, 27, 537, DateTimeKind.Local).AddTicks(4609), "AQAAAAIAAYagAAAAEMJISL50U2Cm6RVYBr92VYA5+97zEoo/8xuE5J6Lkmw1oavdSiaNyEaGT6lOocGirQ==", "a4e048b1-666e-4093-a53c-857979a128e2", new DateTime(2024, 4, 1, 0, 6, 27, 537, DateTimeKind.Local).AddTicks(4622) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "58fbedfc-e682-479b-ba46-19ef4c137d2a",
                columns: new[] { "ConcurrencyStamp", "Creation", "PasswordHash", "SecurityStamp", "Update" },
                values: new object[] { "78cdaedf-5e32-49c8-8137-b33d107e9334", new DateTime(2024, 4, 1, 0, 6, 27, 377, DateTimeKind.Local).AddTicks(1840), "AQAAAAIAAYagAAAAEEbg86FvCQmgCmoPx7tnH8Z4MFSsnipu5AKYvbdrvbVGp5/I/0XkjjLHwV/ISW1XSQ==", "f4315ebe-5aa4-4693-92f8-760fa30bcf1e", new DateTime(2024, 4, 1, 0, 6, 27, 377, DateTimeKind.Local).AddTicks(1861) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c2ee6493-5a73-46f3-a3f2-46d1d11d7176",
                columns: new[] { "ConcurrencyStamp", "Creation", "PasswordHash", "SecurityStamp", "Update" },
                values: new object[] { "3e5f083a-2355-4d9b-928a-cfe58d1cef3c", new DateTime(2024, 4, 1, 0, 6, 27, 69, DateTimeKind.Local).AddTicks(4677), "AQAAAAIAAYagAAAAEPl1JxEEAeOJ+iY2BX425JDgh0UvWCBqCMKzn3b/kihsREcDPZAGPf1LsLGes3aB0A==", "67071d7f-0a28-4661-a913-64bc01bf0c0a", new DateTime(2024, 4, 1, 0, 6, 27, 69, DateTimeKind.Local).AddTicks(4680) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e0765c93-676c-4199-b7ee-d7877c471821",
                columns: new[] { "ConcurrencyStamp", "Creation", "PasswordHash", "SecurityStamp", "Update" },
                values: new object[] { "f0b11bfc-b55c-447c-aada-8bf881df7030", new DateTime(2024, 4, 1, 0, 6, 27, 246, DateTimeKind.Local).AddTicks(8248), "AQAAAAIAAYagAAAAEOcsraTXmDSZT+bVrLpJPpI5TL+UQUs550DwubyAz5hLnn7merchCClgms9Ug5IEqw==", "fb1e4432-a51a-46c4-b690-ee42376f715c", new DateTime(2024, 4, 1, 0, 6, 27, 246, DateTimeKind.Local).AddTicks(8265) });

            migrationBuilder.UpdateData(
                table: "City",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6838), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6841) });

            migrationBuilder.UpdateData(
                table: "City",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6844), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6844) });

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6579), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6597) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7005), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7008) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7023), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7023) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7028), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7029) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7034), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7034) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7039), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7040) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7042), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7043) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7047), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7048) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7051), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7052) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7055), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7055) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7059), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7059) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7063), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7064) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7066), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7067) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7071), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7071) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7074), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7074) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7078), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7078) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7082), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7084) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7146), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7149) });

            migrationBuilder.UpdateData(
                table: "Estate",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7155), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(7156) });

            migrationBuilder.UpdateData(
                table: "Owner",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6920), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6921) });

            migrationBuilder.UpdateData(
                table: "Owner",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6923), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6924) });

            migrationBuilder.UpdateData(
                table: "Owner",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6927), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6927) });

            migrationBuilder.UpdateData(
                table: "Province",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6755), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6756) });

            migrationBuilder.UpdateData(
                table: "Province",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Creation", "Update" },
                values: new object[] { new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6758), new DateTime(2024, 4, 1, 0, 6, 27, 652, DateTimeKind.Local).AddTicks(6761) });
        }
    }
}
