using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Species",
                columns: table => new
                {
                    ColId = table.Column<string>(type: "TEXT", nullable: false),
                    ScientificName = table.Column<string>(type: "TEXT", nullable: false),
                    Rank = table.Column<string>(type: "TEXT", nullable: false),
                    Genus = table.Column<string>(type: "TEXT", nullable: true),
                    Family = table.Column<string>(type: "TEXT", nullable: true),
                    Order = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Phylum = table.Column<string>(type: "TEXT", nullable: false),
                    IsExtinct = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species", x => x.ColId);
                });

            migrationBuilder.CreateTable(
                name: "Distributions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CatalogueOfLifeId = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    SpeciesColId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Distributions_Species_SpeciesColId",
                        column: x => x.SpeciesColId,
                        principalTable: "Species",
                        principalColumn: "ColId");
                });

            migrationBuilder.CreateTable(
                name: "VernacularNames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CatalogueOfLifeId = table.Column<string>(type: "TEXT", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SpeciesColId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VernacularNames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VernacularNames_Species_SpeciesColId",
                        column: x => x.SpeciesColId,
                        principalTable: "Species",
                        principalColumn: "ColId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Distributions_CatalogueOfLifeId",
                table: "Distributions",
                column: "CatalogueOfLifeId");

            migrationBuilder.CreateIndex(
                name: "IX_Distributions_SpeciesColId",
                table: "Distributions",
                column: "SpeciesColId");

            migrationBuilder.CreateIndex(
                name: "IX_VernacularNames_CatalogueOfLifeId",
                table: "VernacularNames",
                column: "CatalogueOfLifeId");

            migrationBuilder.CreateIndex(
                name: "IX_VernacularNames_SpeciesColId",
                table: "VernacularNames",
                column: "SpeciesColId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Distributions");

            migrationBuilder.DropTable(
                name: "VernacularNames");

            migrationBuilder.DropTable(
                name: "Species");
        }
    }
}
