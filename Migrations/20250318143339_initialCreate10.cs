using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class initialCreate10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Adresses_AdressId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "AdressId",
                table: "Orders",
                newName: "AdressAddressElementId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_AdressId",
                table: "Orders",
                newName: "IX_Orders_AdressAddressElementId");

            migrationBuilder.CreateTable(
                name: "AddressElements",
                columns: table => new
                {
                    AddressElementId = table.Column<Guid>(type: "uuid", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Street = table.Column<string>(type: "text", nullable: false),
                    House = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressElements", x => x.AddressElementId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AddressElements_AdressAddressElementId",
                table: "Orders",
                column: "AdressAddressElementId",
                principalTable: "AddressElements",
                principalColumn: "AddressElementId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AddressElements_AdressAddressElementId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "AddressElements");

            migrationBuilder.RenameColumn(
                name: "AdressAddressElementId",
                table: "Orders",
                newName: "AdressId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_AdressAddressElementId",
                table: "Orders",
                newName: "IX_Orders_AdressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Adresses_AdressId",
                table: "Orders",
                column: "AdressId",
                principalTable: "Adresses",
                principalColumn: "AdressId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
