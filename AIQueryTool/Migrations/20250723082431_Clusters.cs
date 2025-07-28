using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIToolbox.Migrations
{
    /// <inheritdoc />
    public partial class Clusters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClusterMethod",
                table: "StoredChunks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClusterNumber",
                table: "StoredChunks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Rules",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rules", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rules");

            migrationBuilder.DropColumn(
                name: "ClusterMethod",
                table: "StoredChunks");

            migrationBuilder.DropColumn(
                name: "ClusterNumber",
                table: "StoredChunks");
        }
    }
}
