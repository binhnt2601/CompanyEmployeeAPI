using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyEmployees.Migrations
{
    public partial class AddRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "7a408171-e9d8-42d6-93ff-f71b3a26a19e", "790c08ae-be6e-4631-986f-a30134b05da7", "Manager", "MANAGER" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "87918b31-5e1b-4852-a311-39a2dd36d9c7", "3cfbc731-4a02-4caa-a944-02ab07a72c4b", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "7a408171-e9d8-42d6-93ff-f71b3a26a19e");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "87918b31-5e1b-4852-a311-39a2dd36d9c7");
        }
    }
}
