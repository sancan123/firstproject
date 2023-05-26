using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingMallSys.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "NETCORE");

            migrationBuilder.CreateTable(
                name: "STUDENT",
                schema: "NETCORE",
                columns: table => new
                {
                    USERID = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STUDENT", x => x.USERID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "STUDENT",
                schema: "NETCORE");
        }
    }
}
