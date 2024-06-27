using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Project.Migrations
{
    /// <inheritdoc />
    public partial class Addingvalues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Software",
                columns: new[] { "Id", "Category", "CurrentVersion", "Description", "IsOneTimePurchase", "IsSubscriptionPurchase", "Name", "OneTimePrice", "SubscriptionPrice" },
                values: new object[,]
                {
                    { 1, "Antivirus", "1.0", "Antivirus software", true, false, "Kaspersky", 1000m, null },
                    { 2, "Office", "1.0", "Office software", true, false, "Microsoft Office", 300m, null }
                });

            migrationBuilder.InsertData(
                table: "Discount",
                columns: new[] { "Id", "DiscountFrom", "Name", "PercentageValue", "Type", "DiscountUntil", "SoftwareId" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Student", 50m, "Agreement", new DateTime(2024, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sale", 30m, "Agreement", new DateTime(2024, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 3, new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Summer", 80m, "Agreement", new DateTime(2022, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Discount",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Discount",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Discount",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Software",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Software",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
