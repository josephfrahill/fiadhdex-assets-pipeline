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
                name: "Taxa",
                columns: table => new
                {
                    ColId = table.Column<string>(type: "TEXT", nullable: false),
                    ScientificName = table.Column<string>(type: "TEXT", nullable: false),
                    Rank = table.Column<string>(type: "TEXT", nullable: false),
                    Genus = table.Column<string>(type: "TEXT", nullable: true),
                    Family = table.Column<string>(type: "TEXT", nullable: true),
                    Order = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    SubPhylum = table.Column<string>(type: "TEXT", nullable: true),
                    Phylum = table.Column<string>(type: "TEXT", nullable: false),
                    IsExtinct = table.Column<string>(type: "TEXT", nullable: true),
                    ExternalExtantVerified = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taxa", x => x.ColId);
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
                    TaxonColId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Distributions_Taxa_TaxonColId",
                        column: x => x.TaxonColId,
                        principalTable: "Taxa",
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
                    TaxonColId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VernacularNames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VernacularNames_Taxa_TaxonColId",
                        column: x => x.TaxonColId,
                        principalTable: "Taxa",
                        principalColumn: "ColId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Distributions_CatalogueOfLifeId",
                table: "Distributions",
                column: "CatalogueOfLifeId");

            migrationBuilder.CreateIndex(
                name: "IX_Distributions_TaxonColId",
                table: "Distributions",
                column: "TaxonColId");

            migrationBuilder.CreateIndex(
                name: "IX_VernacularNames_CatalogueOfLifeId",
                table: "VernacularNames",
                column: "CatalogueOfLifeId");

            migrationBuilder.CreateIndex(
                name: "IX_VernacularNames_TaxonColId",
                table: "VernacularNames",
                column: "TaxonColId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Distributions");

            migrationBuilder.DropTable(
                name: "VernacularNames");

            migrationBuilder.DropTable(
                name: "Taxa");
        }
    }
}
