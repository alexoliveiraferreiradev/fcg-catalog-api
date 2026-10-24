using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fcg.Catalog.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGameCoverUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverUrl",
                table: "Games",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverUrl",
                table: "Games");
        }
    }
}
