using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taller_1_IDWM.Migrations.Data
{
    /// <inheritdoc />
    public partial class RecepitsChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(
                name: "PK_ReceiptProducts",
                table: "ReceiptProducts",
                columns: new[] { "ReceiptId", "ProductId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ReceiptProducts",
                table: "ReceiptProducts");
        }
    }
}
