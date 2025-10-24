using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentalSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class edit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Cars_CarId",
                table: "Images");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Images",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "CarType",
                table: "Cars");

            migrationBuilder.RenameTable(
                name: "Images",
                newName: "CarImages");

            migrationBuilder.RenameIndex(
                name: "IX_Images_CarId",
                table: "CarImages",
                newName: "IX_CarImages_CarId");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Cars",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CarImages",
                table: "CarImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CarImages_Cars_CarId",
                table: "CarImages",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarImages_Cars_CarId",
                table: "CarImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CarImages",
                table: "CarImages");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Cars");

            migrationBuilder.RenameTable(
                name: "CarImages",
                newName: "Images");

            migrationBuilder.RenameIndex(
                name: "IX_CarImages_CarId",
                table: "Images",
                newName: "IX_Images_CarId");

            migrationBuilder.AddColumn<string>(
                name: "CarType",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Images",
                table: "Images",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Cars_CarId",
                table: "Images",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
