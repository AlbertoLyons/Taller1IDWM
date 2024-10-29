using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taller_1_IDWM.Migrations.Data
{
    /// <inheritdoc />
    public partial class ProductsUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Products",
                newName: "ImageUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Products",
                newName: "Image");
        }
    }
}
