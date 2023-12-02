using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MS.Services.Identity.Shared.Data.Migrations.Identity
{
    public partial class AddUserManagmentId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "user_managment_id",
                table: "asp_net_users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "user_managment_id",
                table: "asp_net_users");
        }
    }
}
