using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CP5.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_cp5_estado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_cp5_estado", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_cp5_usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Password = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Role = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_cp5_usuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_cp5_cliente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    Idade = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    EstadoId = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_cp5_cliente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_cp5_cliente_tb_cp5_estado_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "tb_cp5_estado",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "tb_cp5_curso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ClienteId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ClienteEntityId = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_cp5_curso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_cp5_curso_tb_cp5_cliente_ClienteEntityId",
                        column: x => x.ClienteEntityId,
                        principalTable: "tb_cp5_cliente",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "tb_cp5_produto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ClienteId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ClienteEntityId = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_cp5_produto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_cp5_produto_tb_cp5_cliente_ClienteEntityId",
                        column: x => x.ClienteEntityId,
                        principalTable: "tb_cp5_cliente",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IDX_cp5_cliente_email",
                table: "tb_cp5_cliente",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_cp5_cliente_Email_Nome",
                table: "tb_cp5_cliente",
                columns: new[] { "Email", "Nome" });

            migrationBuilder.CreateIndex(
                name: "IX_tb_cp5_cliente_EstadoId",
                table: "tb_cp5_cliente",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_cp5_curso_ClienteEntityId",
                table: "tb_cp5_curso",
                column: "ClienteEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_cp5_produto_ClienteEntityId",
                table: "tb_cp5_produto",
                column: "ClienteEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_cp5_curso");

            migrationBuilder.DropTable(
                name: "tb_cp5_produto");

            migrationBuilder.DropTable(
                name: "tb_cp5_usuario");

            migrationBuilder.DropTable(
                name: "tb_cp5_cliente");

            migrationBuilder.DropTable(
                name: "tb_cp5_estado");
        }
    }
}
