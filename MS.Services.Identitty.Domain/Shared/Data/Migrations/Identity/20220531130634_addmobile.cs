using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MS.Services.Identity.Shared.Data.Migrations.Identity
{
    public partial class addmobile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "mobile_number",
                table: "asp_net_users",
                type: "character varying(11)",
                maxLength: 11,
                nullable: true,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "verify_code",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mobile_number = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    verify_code_value = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expire_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_verify_code", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_verify_code_id",
                table: "verify_code",
                column: "id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "verify_code");

            migrationBuilder.DropColumn(
                name: "mobile_number",
                table: "asp_net_users");
        }
    }
}
