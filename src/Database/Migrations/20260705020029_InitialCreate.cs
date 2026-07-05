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
                    CatalogueOfLifeId = table.Column<string>(type: "TEXT", nullable: false),
                    ScientificName = table.Column<string>(type: "TEXT", nullable: false),
                    Rank = table.Column<string>(type: "TEXT", nullable: false),
                    Class = table.Column<string>(type: "TEXT", nullable: true),
                    Order = table.Column<string>(type: "TEXT", nullable: true),
                    Family = table.Column<string>(type: "TEXT", nullable: true),
                    Genus = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species", x => x.CatalogueOfLifeId);
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
                    SpeciesCatalogueOfLifeId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Distributions_Species_SpeciesCatalogueOfLifeId",
                        column: x => x.SpeciesCatalogueOfLifeId,
                        principalTable: "Species",
                        principalColumn: "CatalogueOfLifeId");
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
                    SpeciesCatalogueOfLifeId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VernacularNames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VernacularNames_Species_SpeciesCatalogueOfLifeId",
                        column: x => x.SpeciesCatalogueOfLifeId,
                        principalTable: "Species",
                        principalColumn: "CatalogueOfLifeId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Distributions_CatalogueOfLifeId",
                table: "Distributions",
                column: "CatalogueOfLifeId");

            migrationBuilder.CreateIndex(
                name: "IX_Distributions_SpeciesCatalogueOfLifeId",
                table: "Distributions",
                column: "SpeciesCatalogueOfLifeId");

            migrationBuilder.CreateIndex(
                name: "IX_VernacularNames_CatalogueOfLifeId",
                table: "VernacularNames",
                column: "CatalogueOfLifeId");

            migrationBuilder.CreateIndex(
                name: "IX_VernacularNames_SpeciesCatalogueOfLifeId",
                table: "VernacularNames",
                column: "SpeciesCatalogueOfLifeId");
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
