using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Курсач_1.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSendEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSendEmail",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSendEmail",
                table: "Users");
        }
    }
}
