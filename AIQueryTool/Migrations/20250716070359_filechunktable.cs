using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Pgvector;

#nullable disable

namespace AIToolbox.Migrations
{
    /// <inheritdoc />
    public partial class filechunktable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Embeddings",
                table: "StoredFiles");

            migrationBuilder.AddColumn<DateTime>(
                name: "uploadTime",
                table: "StoredFiles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "StoredChunks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileId = table.Column<int>(type: "integer", nullable: false),
                    ChunkContent = table.Column<string>(type: "text", nullable: false),
                    Embeddings = table.Column<Vector>(type: "vector(768)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredChunks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoredChunks_StoredFiles_FileId",
                        column: x => x.FileId,
                        principalTable: "StoredFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoredChunks_FileId",
                table: "StoredChunks",
                column: "FileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoredChunks");

            migrationBuilder.DropColumn(
                name: "uploadTime",
                table: "StoredFiles");

            migrationBuilder.AddColumn<Vector>(
                name: "Embeddings",
                table: "StoredFiles",
                type: "vector(768)",
                nullable: false);
        }
    }
}
