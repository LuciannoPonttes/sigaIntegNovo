using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SigaDocIntegracao.Web.Migrations
{
    /// <inheritdoc />
    public partial class Add_Hist1809v11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CAD_MODELO_EMAIL_HIST",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TXT_DOCUMENTO_NOME = table.Column<string>(type: "text", nullable: false),
                    FLG_STATUS_ENVIADO = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CAD_MODELO_EMAIL_HIST", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CAD_MODELO_NOTIF_EMAIL",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_SIGA_DOC = table.Column<string>(type: "text", nullable: false),
                    TXT_NOME_NOT = table.Column<string>(type: "text", nullable: false),
                    TXT_DESCRICAO_MODELO = table.Column<string>(type: "text", nullable: false),
                    TXT_DESTINATARIOS_EMAILS = table.Column<string>(type: "text", nullable: false),
                    TXT_CONTEUDO_EMAIL = table.Column<string>(type: "text", nullable: false),
                    TXT_ASSUNTO = table.Column<string>(type: "text", nullable: false),
                    FLG_STATUS_ATIVO = table.Column<bool>(type: "boolean", nullable: false),
                    DTH_INICIO_ENVIO = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DTH_ULTIMO_PROCESSAMENTO = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CAD_MODELO_NOTIF_EMAIL", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CAD_MODELO_EMAIL_HIST");

            migrationBuilder.DropTable(
                name: "CAD_MODELO_NOTIF_EMAIL");
        }
    }
}
