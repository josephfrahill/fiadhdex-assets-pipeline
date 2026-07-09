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
                    Genus = table.Column<string>(type: "TEXT", nullable: false),
                    Family = table.Column<string>(type: "TEXT", nullable: false),
                    Order = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    SubPhylum = table.Column<string>(type: "TEXT", nullable: false),
                    Phylum = table.Column<string>(type: "TEXT", nullable: false),
                    IsExtinct = table.Column<string>(type: "TEXT", nullable: true),
                    ExternalExtantVerified = table.Column<string>(type: "TEXT", nullable: true)
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
                    ColId = table.Column<string>(type: "TEXT", nullable: false),
                    Area = table.Column<string>(type: "TEXT", nullable: false),
                    EstablishmentMeans = table.Column<string>(type: "TEXT", nullable: true),
                    DegreeOfEstablishment = table.Column<string>(type: "TEXT", nullable: true),
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
                    ColId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Transliteration = table.Column<string>(type: "TEXT", nullable: true),
                    Language = table.Column<string>(type: "TEXT", nullable: true),
                    Preferred = table.Column<bool>(type: "INTEGER", nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: true),
                    Area = table.Column<string>(type: "TEXT", nullable: true),
                    Merged = table.Column<bool>(type: "INTEGER", nullable: false),
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
                name: "IX_Distributions_ColId",
                table: "Distributions",
                column: "ColId");

            migrationBuilder.CreateIndex(
                name: "IX_Distributions_TaxonColId",
                table: "Distributions",
                column: "TaxonColId");

            migrationBuilder.CreateIndex(
                name: "IX_VernacularNames_ColId",
                table: "VernacularNames",
                column: "ColId");

            migrationBuilder.CreateIndex(
                name: "IX_VernacularNames_Language",
                table: "VernacularNames",
                column: "Language");

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
