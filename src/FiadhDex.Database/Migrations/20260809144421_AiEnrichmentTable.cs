using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AiEnrichmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AiEnrichments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ColId = table.Column<string>(type: "TEXT", nullable: false),
                    Model = table.Column<string>(type: "TEXT", nullable: false),
                    PromptVersion = table.Column<string>(type: "TEXT", nullable: false),
                    GeneratedAt = table.Column<string>(type: "TEXT", nullable: false),
                    Data = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiEnrichments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AiEnrichments_Taxa_ColId",
                        column: x => x.ColId,
                        principalTable: "Taxa",
                        principalColumn: "ColId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AiEnrichments_ColId",
                table: "AiEnrichments",
                column: "ColId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AiEnrichments_ColId_Data",
                table: "AiEnrichments",
                columns: new[] { "ColId", "Data" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AiEnrichments");
        }
    }
}
