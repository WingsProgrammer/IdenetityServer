using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MS.Services.Identity.Shared.Data.Migrations.Identity
{
    public partial class addUsedToVerifyCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "used",
                table: "verify_code",
                type: "boolean",
                nullable: false,
                defaultValueSql: "false");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "used",
                table: "verify_code");
        }
    }
}
