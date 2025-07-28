using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIToolbox.Migrations
{
    /// <inheritdoc />
    public partial class TodoUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "TodoItems",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_userId",
                table: "TodoItems",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_AspNetUsers_userId",
                table: "TodoItems",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_AspNetUsers_userId",
                table: "TodoItems");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_userId",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "TodoItems");
        }
    }
}
