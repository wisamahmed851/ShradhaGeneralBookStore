using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShradhaGeneralBookStore.Migrations
{
    /// <inheritdoc />
    public partial class AddImageTypeToProductImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageType",
                table: "ProductImage",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageType",
                table: "ProductImage");
        }
    }
}
