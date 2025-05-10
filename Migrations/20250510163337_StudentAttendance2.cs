using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace system_university.Migrations
{
    /// <inheritdoc />
    public partial class StudentAttendance2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScanDate",
                table: "StudentAttendances");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "StudentAttendances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "StudentAttendances");

            migrationBuilder.AddColumn<DateTime>(
                name: "ScanDate",
                table: "StudentAttendances",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
