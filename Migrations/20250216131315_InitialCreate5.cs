using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Orders_OrderId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_OrderId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProductsCount",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "ProductsCount",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductsCount",
                table: "ProductsCount",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsCount_OrderId",
                table: "ProductsCount",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductsCount_Orders_OrderId",
                table: "ProductsCount",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductsCount_Orders_OrderId",
                table: "ProductsCount");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductsCount",
                table: "ProductsCount");

            migrationBuilder.DropIndex(
                name: "IX_ProductsCount_OrderId",
                table: "ProductsCount");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProductsCount");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "ProductsCount");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Products",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_OrderId",
                table: "Products",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Orders_OrderId",
                table: "Products",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId");
        }
    }
}
