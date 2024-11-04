using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KiloMart.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddingUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "Role",
                table: "AspNetUsers",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "AspNetUsers");
        }
    }
}
