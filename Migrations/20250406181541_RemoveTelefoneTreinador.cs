using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APITATT1.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTelefoneTreinador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "telefone",
                table: "Treinadores");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Treinadores",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Especialidade",
                table: "Treinadores",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Treinadores");

            migrationBuilder.DropColumn(
                name: "Especialidade",
                table: "Treinadores");

            migrationBuilder.AddColumn<int>(
                name: "telefone",
                table: "Treinadores",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
