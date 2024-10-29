using Microsoft.EntityFrameworkCore.Migrations;
using MyMovies.Utility;

#nullable disable

namespace MyMovies.Migrations
{
    /// <inheritdoc />
    public partial class SeadRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              table: "AspNetRoles",
              columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
              values: new object[] { Guid.NewGuid().ToString(), SD.Role_User, SD.Role_User.ToUpper(), Guid.NewGuid().ToString() }
              );

            migrationBuilder.InsertData(
           table: "AspNetRoles",
           columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
           values: new object[] { Guid.NewGuid().ToString(), SD.Role_Admin, SD.Role_Admin.ToUpper(), Guid.NewGuid().ToString() }
           );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [AspNetRoles]");

        }
    }
}
