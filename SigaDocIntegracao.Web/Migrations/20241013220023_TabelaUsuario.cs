using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigaDocIntegracao.Web.Migrations
{
    /// <inheritdoc />
    public partial class TabelaUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_permissao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<bool>(type: "boolean", nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_permissao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_usuario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<bool>(type: "boolean", nullable: false),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    NomeNormalizado = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Matricula = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    StatusCadastro = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_usuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissaoUsuario",
                columns: table => new
                {
                    PermissoesId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuariosId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissaoUsuario", x => new { x.PermissoesId, x.UsuariosId });
                    table.ForeignKey(
                        name: "FK_PermissaoUsuario_tb_permissao_PermissoesId",
                        column: x => x.PermissoesId,
                        principalTable: "tb_permissao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermissaoUsuario_tb_usuario_UsuariosId",
                        column: x => x.UsuariosId,
                        principalTable: "tb_usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_usuario_permissao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<bool>(type: "boolean", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permissao_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_usuario_permissao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_usuario_permissao_tb_permissao_permissao_id",
                        column: x => x.permissao_id,
                        principalTable: "tb_permissao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tb_usuario_permissao_tb_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "tb_usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PermissaoUsuario_UsuariosId",
                table: "PermissaoUsuario",
                column: "UsuariosId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_usuario_permissao_permissao_id",
                table: "tb_usuario_permissao",
                column: "permissao_id");

            migrationBuilder.CreateIndex(
                name: "IX_tb_usuario_permissao_usuario_id",
                table: "tb_usuario_permissao",
                column: "usuario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PermissaoUsuario");

            migrationBuilder.DropTable(
                name: "tb_usuario_permissao");

            migrationBuilder.DropTable(
                name: "tb_permissao");

            migrationBuilder.DropTable(
                name: "tb_usuario");
        }
    }
}
