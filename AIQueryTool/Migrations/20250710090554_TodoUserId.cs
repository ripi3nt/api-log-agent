using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIToolbox.Migrations
{
    /// <inheritdoc />
    public partial class TodoUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_AspNetUsers_userId",
                table: "TodoItems");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_userId",
                table: "TodoItems");

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "TodoItems",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "TodoItems",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

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
    }
}
