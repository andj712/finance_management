using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace finance_management.Migrations
{
    /// <inheritdoc />
    public partial class AddSplitTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "Transactions",
                type: "double precision",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.CreateTable(
                name: "Splits",
                columns: table => new
                {
                    TransactionId = table.Column<string>(type: "character varying(15)", nullable: false),
                    CatCode = table.Column<string>(type: "character varying(10)", nullable: false),
                    Amount = table.Column<double>(type: "double precision", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Splits", x => new { x.TransactionId, x.CatCode });
                    table.ForeignKey(
                        name: "FK_Splits_Categories_CatCode",
                        column: x => x.CatCode,
                        principalTable: "Categories",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Splits_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Splits_CatCode",
                table: "Splits",
                column: "CatCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Splits");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Transactions",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldPrecision: 18,
                oldScale: 2);
        }
    }
}
