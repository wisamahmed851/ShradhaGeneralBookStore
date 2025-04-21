using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShradhaGeneralBookStore.Migrations
{
    /// <inheritdoc />
    public partial class Feedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "AQAAAAIAAYagAAAAEM+fExcaDSugZ1EnuJv9N/cmcMLvPwQ7JBrH/3aSd5gBqUyENfh4hWXufEYCWhGyhg==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "123456789");
        }
    }
}
