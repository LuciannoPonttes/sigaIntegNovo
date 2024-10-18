using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigaDocIntegracao.Web.Migrations
{
    /// <inheritdoc />
    public partial class Add_Hist2409 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DTH_CRIACAO_NOT",
                table: "CAD_MODELO_NOTIF_EMAIL",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DTH_CRIACAO_NOT",
                table: "CAD_MODELO_NOTIF_EMAIL");
        }
    }
}
