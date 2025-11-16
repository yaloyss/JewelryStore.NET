using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Catalog.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DataSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "categories",
                columns: new[] { "categoryid", "name" },
                values: new object[,]
                {
                    { 1, "Rings" },
                    { 2, "Earrings" },
                    { 3, "Pendants" },
                    { 4, "Bracelets" },
                    { 5, "Necklaces" }
                });

            migrationBuilder.InsertData(
                table: "metals",
                columns: new[] { "metalid", "color", "name" },
                values: new object[,]
                {
                    { 1, "Yellow", "Gold" },
                    { 2, "White", "Gold" },
                    { 3, "Rose", "Gold" },
                    { 4, "Silver", "Silver" },
                    { 5, "White", "Platinum" }
                });

            migrationBuilder.InsertData(
                table: "stones",
                columns: new[] { "stoneid", "name" },
                values: new object[,]
                {
                    { 1, "Diamond" },
                    { 2, "Ruby" },
                    { 3, "Emerald" },
                    { 4, "Moonstone" },
                    { 5, "Amethyst" },
                    { 6, "Garnet" },
                    { 7, "Opal" },
                    { 8, "Pearl" },
                    { 9, "Cubic Zirconia" },
                    { 10, "Onyx" },
                    { 11, "Smoky Quartz" }
                });

            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "productid", "categoryid", "manufacturer", "metalid", "name", "price", "size", "weight" },
                values: new object[,]
                {
                    { 1, 1, "Ukraine", 2, "White Gold Ring with Diamond", 28000m, 16.5m, 3.2m },
                    { 2, 1, "Ukraine", 5, "Platinum Smoky Quartz Ring", 32000m, 16.5m, 4.5m },
                    { 3, 1, "Ukraine", 4, "Silver Amethyst Ring", 3100m, 17.5m, 3.8m },
                    { 4, 1, "Ukraine", 4, "Silver Ring with Cubic Zirconia", 1200m, 18m, 8.5m },
                    { 5, 2, "Ukraine", 2, "Diamond Stud Earrings", 28000m, null, 2.1m },
                    { 6, 2, "Ukraine", 2, "Emerald Earrings", 43000m, null, 6.3m },
                    { 7, 2, "Ukraine", 4, "Silver Hoop Earrings with Cubic Zirconia", 3500m, null, 4.2m },
                    { 8, 2, "Ukraine", 1, "Pearl Earrings", 12000m, null, 3.5m },
                    { 9, 3, "Ukraine", 4, "Cross Pendant with Onyx", 6600m, 2.5m, 1.8m },
                    { 10, 3, "Ukraine", 4, "Heart Pendant with Moonstone", 2150m, 2.0m, 2.3m },
                    { 11, 3, "Ukraine", 4, "Smoky Quartz Pendant", 9900m, 1.8m, 1.5m },
                    { 12, 3, "Ukraine", 4, "Garnet Pendant", 2000m, 1.5m, 1.2m },
                    { 13, 4, "Ukraine", 4, "Silver Cross Bracelet with Onyx", 8900m, 19m, 15.5m },
                    { 14, 4, "Ukraine", 4, "Moonstone and Opal Bracelet", 23000m, 18m, 12.4m },
                    { 15, 4, "Ukraine", 5, "Thin Chain Bracelet", 5100m, 18m, 3.1m },
                    { 16, 4, "Ukraine", 2, "Pearl Bracelet", 26000m, 17.5m, 9.8m },
                    { 17, 5, "Ukraine", null, "Pearl Necklace", 25000m, 45m, 45.0m },
                    { 18, 5, "Ukraine", 4, "Garnet Necklace", 18500m, 50m, 52.0m },
                    { 19, 5, "Ukraine", 4, "Diamond Necklace", 51500m, 48m, 38.5m }
                });

            migrationBuilder.InsertData(
                table: "product_stone",
                columns: new[] { "productid", "stoneid" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 11 },
                    { 3, 5 },
                    { 4, 9 },
                    { 5, 1 },
                    { 6, 3 },
                    { 7, 9 },
                    { 8, 8 },
                    { 9, 10 },
                    { 10, 4 },
                    { 11, 11 },
                    { 12, 6 },
                    { 13, 10 },
                    { 14, 4 },
                    { 14, 7 },
                    { 16, 8 },
                    { 17, 8 },
                    { 18, 6 },
                    { 19, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "metals",
                keyColumn: "metalid",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "product_stone",
                keyColumns: new[] { "productid", "stoneid" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "product_stone",
                keyColumns: new[] { "productid", "stoneid" },
                keyValues: new object[] { 2, 11 });

            migrationBuilder.DeleteData(
                table: "product_stone",
                keyColumns: new[] { "productid", "stoneid" },
                keyValues: new object[] { 3, 5 });

            migrationBuilder.DeleteData(
                table: "product_stone",
                keyColumns: new[] { "productid", "stoneid" },
                keyValues: new object[] { 4, 9 });

            migrationBuilder.DeleteData(
                table: "product_stone",
                keyColumns: new[] { "productid", "stoneid" },
                keyValues: new object[] { 5, 1 });

            migrationBuilder.DeleteData(
                table: "product_stone",
                keyColumns: new[] { "productid", "stoneid" },
                keyValues: new object[] { 6, 3 });

            migrationBuilder.DeleteData(
                table: "product_stone",
                keyColumns: new[] { "productid", "stoneid" },
                keyValues: new object[] { 7, 9 });

            migrationBuilder.DeleteData(
                table: "product_stone",
                keyColumns: new[] { "productid", "stoneid" },
                keyValues: new object[] { 8, 8 });

            migrationBuilder.DeleteData(
                table: "product_stone",
                keyColumns: new[] { "productid", "stoneid" },
                keyValues: new object[] { 9, 10 });

            migrationBuilder.DeleteData(
                table: "product_stone",
                keyColumns: new[] { "productid", "stoneid" },
                keyValues: new object[] { 10, 4 });

            migrationBuilder.DeleteData(
                table: "product_stone",
                keyColumns: new[] { "productid", "stoneid" },
                keyValues: new object[] { 11, 11 });

            migrationBuilder.DeleteData(
                table: "product_stone",
                keyColumns: new[] { "productid", "stoneid" },
                keyValues: new object[] { 12, 6 });

            migrationBuilder.DeleteData(
                table: "product_stone",
                keyColumns: new[] { "productid", "stoneid" },
                keyValues: new object[] { 13, 10 });

            migrationBuilder.DeleteData(
                table: "product_stone",
                keyColumns: new[] { "productid", "stoneid" },
                keyValues: new object[] { 14, 4 });

            migrationBuilder.DeleteData(
                table: "product_stone",
                keyColumns: new[] { "productid", "stoneid" },
                keyValues: new object[] { 14, 7 });

            migrationBuilder.DeleteData(
                table: "product_stone",
                keyColumns: new[] { "productid", "stoneid" },
                keyValues: new object[] { 16, 8 });

            migrationBuilder.DeleteData(
                table: "product_stone",
                keyColumns: new[] { "productid", "stoneid" },
                keyValues: new object[] { 17, 8 });

            migrationBuilder.DeleteData(
                table: "product_stone",
                keyColumns: new[] { "productid", "stoneid" },
                keyValues: new object[] { 18, 6 });

            migrationBuilder.DeleteData(
                table: "product_stone",
                keyColumns: new[] { "productid", "stoneid" },
                keyValues: new object[] { 19, 1 });

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "productid",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "stones",
                keyColumn: "stoneid",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "productid",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "productid",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "productid",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "productid",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "productid",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "productid",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "productid",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "productid",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "productid",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "productid",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "productid",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "productid",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "productid",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "productid",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "productid",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "productid",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "productid",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "productid",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "stones",
                keyColumn: "stoneid",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "stones",
                keyColumn: "stoneid",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "stones",
                keyColumn: "stoneid",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "stones",
                keyColumn: "stoneid",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "stones",
                keyColumn: "stoneid",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "stones",
                keyColumn: "stoneid",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "stones",
                keyColumn: "stoneid",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "stones",
                keyColumn: "stoneid",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "stones",
                keyColumn: "stoneid",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "stones",
                keyColumn: "stoneid",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "categoryid",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "categoryid",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "categoryid",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "categoryid",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "categoryid",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "metals",
                keyColumn: "metalid",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "metals",
                keyColumn: "metalid",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "metals",
                keyColumn: "metalid",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "metals",
                keyColumn: "metalid",
                keyValue: 5);
        }
    }
}
