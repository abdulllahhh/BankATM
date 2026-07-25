using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bank.Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModelAdjustment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventType",
                table: "AuditLogs",
                newName: "Details");

            migrationBuilder.RenameColumn(
                name: "Data",
                table: "AuditLogs",
                newName: "ActionType");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "AuditLogs",
                newName: "Timestamp");

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "AuditLogs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CorrelationId",
                table: "AuditLogs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "CorrelationId",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "AuditLogs",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "Details",
                table: "AuditLogs",
                newName: "EventType");

            migrationBuilder.RenameColumn(
                name: "ActionType",
                table: "AuditLogs",
                newName: "Data");
        }
    }
}
