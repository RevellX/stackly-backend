using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace stackly.Migrations
{
    /// <inheritdoc />
    public partial class Edited_Category : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Categories_CategoryName",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_CategoryName",
                table: "Items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Categories");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Categories",
                type: "TEXT",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Categories");

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "Items",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Categories",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CategoryName",
                table: "Items",
                column: "CategoryName");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Categories_CategoryName",
                table: "Items",
                column: "CategoryName",
                principalTable: "Categories",
                principalColumn: "Name");
        }
    }
}
