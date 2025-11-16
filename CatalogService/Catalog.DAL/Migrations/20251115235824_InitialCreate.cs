using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Catalog.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    categoryid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("categories_pkey", x => x.categoryid);
                });

            migrationBuilder.CreateTable(
                name: "metals",
                columns: table => new
                {
                    metalid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("metals_pkey", x => x.metalid);
                });

            migrationBuilder.CreateTable(
                name: "stones",
                columns: table => new
                {
                    stoneid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("stones_pkey", x => x.stoneid);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    productid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    weight = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    size = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    manufacturer = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    metalid = table.Column<int>(type: "integer", nullable: true),
                    categoryid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("products_pkey", x => x.productid);
                    table.ForeignKey(
                        name: "fk_products_categories",
                        column: x => x.categoryid,
                        principalTable: "categories",
                        principalColumn: "categoryid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_products_metals",
                        column: x => x.metalid,
                        principalTable: "metals",
                        principalColumn: "metalid",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "product_stone",
                columns: table => new
                {
                    productid = table.Column<int>(type: "integer", nullable: false),
                    stoneid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("product_stone_pkey", x => new { x.productid, x.stoneid });
                    table.ForeignKey(
                        name: "fk_productstone_products",
                        column: x => x.productid,
                        principalTable: "products",
                        principalColumn: "productid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_productstone_stones",
                        column: x => x.stoneid,
                        principalTable: "stones",
                        principalColumn: "stoneid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_categories_name",
                table: "categories",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_metals_name",
                table: "metals",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_product_stone_stoneid",
                table: "product_stone",
                column: "stoneid");

            migrationBuilder.CreateIndex(
                name: "idx_product_category",
                table: "products",
                column: "categoryid");

            migrationBuilder.CreateIndex(
                name: "idx_product_name",
                table: "products",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "idx_product_price",
                table: "products",
                column: "price");

            migrationBuilder.CreateIndex(
                name: "IX_products_metalid",
                table: "products",
                column: "metalid");

            migrationBuilder.CreateIndex(
                name: "IX_stones_name",
                table: "stones",
                column: "name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_stone");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "stones");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "metals");
        }
    }
}
