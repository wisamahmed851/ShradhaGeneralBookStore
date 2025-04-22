using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShradhaGeneralBookStore.Migrations
{
    /// <inheritdoc />
    public partial class FeedbackAlter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminReply",
                table: "FeedBacks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsReplied",
                table: "FeedBacks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminReply",
                table: "FeedBacks");

            migrationBuilder.DropColumn(
                name: "IsReplied",
                table: "FeedBacks");
        }
    }
}
