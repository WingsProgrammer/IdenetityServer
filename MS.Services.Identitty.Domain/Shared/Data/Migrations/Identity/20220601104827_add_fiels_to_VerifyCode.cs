using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MS.Services.Identity.Shared.Data.Migrations.Identity
{
    public partial class add_fiels_to_VerifyCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "send_date",
                table: "verify_code",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "send_status",
                table: "verify_code",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "send_date",
                table: "verify_code");

            migrationBuilder.DropColumn(
                name: "send_status",
                table: "verify_code");
        }
    }
}
