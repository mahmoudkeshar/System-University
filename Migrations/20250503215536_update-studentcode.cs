using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace system_university.Migrations
{
    /// <inheritdoc />
    public partial class updatestudentcode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_StudentId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "AspNetUsers",
                newName: "StudentCode");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_StudentCode",
                table: "AspNetUsers",
                column: "StudentCode",
                unique: true,
                filter: "[StudentCode] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_StudentCode",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "StudentCode",
                table: "AspNetUsers",
                newName: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_StudentId",
                table: "AspNetUsers",
                column: "StudentId",
                unique: true,
                filter: "[StudentId] IS NOT NULL");
        }
    }
}
