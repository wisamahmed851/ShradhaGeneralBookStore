using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShradhaGeneralBookStore.Migrations
{
    /// <inheritdoc />
    public partial class FeedbackAlter2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSeenByUser",
                table: "FeedBacks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSeenByUser",
                table: "FeedBacks");
        }
    }
}
