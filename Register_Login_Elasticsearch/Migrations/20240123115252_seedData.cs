using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Register_Login_Elasticsearch.Migrations
{
    /// <inheritdoc />
    public partial class seedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "DatabaseId",
                keyColumnType: "int",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "DatabaseId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ElasticId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name", "Password", "Surname", "UserName" },
                values: new object[] { "", "n", "n", "m", "o", "i" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyColumnType: "nvarchar(450)",
                keyValue: "");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "DatabaseId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "ElasticId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "DatabaseId");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "DatabaseId", "ElasticId", "Email", "Name", "Password", "Surname", "UserName" },
                values: new object[] { 1, "a", "n", "n", "m", "o", "i" });
        }
    }
}
