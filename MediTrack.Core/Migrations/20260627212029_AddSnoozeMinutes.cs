using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediTrack.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddSnoozeMinutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Medications",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Frequency",
                table: "Medications",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Medications",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOngoing",
                table: "Medications",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LowStockAlertAt",
                table: "Medications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MealTiming",
                table: "Medications",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "MedicineType",
                table: "Medications",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "RemainingPills",
                table: "Medications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReminderTimes",
                table: "Medications",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "SnoozeMinutes",
                table: "Medications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Medications",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "IsOngoing",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "LowStockAlertAt",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "MealTiming",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "MedicineType",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "RemainingPills",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "ReminderTimes",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "SnoozeMinutes",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Medications");
        }
    }
}
