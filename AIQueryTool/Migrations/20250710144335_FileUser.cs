using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIToolbox.Migrations
{
    /// <inheritdoc />
    public partial class FileUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "userId",
                table: "StoredFiles",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "fileName",
                table: "StoredFiles",
                newName: "FileName");

            migrationBuilder.RenameColumn(
                name: "fileContent",
                table: "StoredFiles",
                newName: "FileContent");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "StoredFiles",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "StoredFiles",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "StoredFiles",
                newName: "fileName");

            migrationBuilder.RenameColumn(
                name: "FileContent",
                table: "StoredFiles",
                newName: "fileContent");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "StoredFiles",
                newName: "id");
        }
    }
}
