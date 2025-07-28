using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIToolbox.Migrations
{
    /// <inheritdoc />
    public partial class addEmbedding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float[]>(
                name: "Embeddings",
                table: "StoredFiles",
                type: "real[]",
                nullable: false,
                defaultValue: new float[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Embeddings",
                table: "StoredFiles");
        }
    }
}
