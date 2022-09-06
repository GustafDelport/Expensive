using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Expense_Tracker.Migrations
{
    public partial class IdName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactions_categories_CategoryID",
                table: "transactions");

            migrationBuilder.RenameColumn(
                name: "CategoryID",
                table: "transactions",
                newName: "CategoryId");

            migrationBuilder.RenameColumn(
                name: "TransactionID",
                table: "transactions",
                newName: "TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_CategoryID",
                table: "transactions",
                newName: "IX_transactions_CategoryId");

            migrationBuilder.RenameColumn(
                name: "CategoryID",
                table: "categories",
                newName: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_categories_CategoryId",
                table: "transactions",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactions_categories_CategoryId",
                table: "transactions");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "transactions",
                newName: "CategoryID");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "transactions",
                newName: "TransactionID");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_CategoryId",
                table: "transactions",
                newName: "IX_transactions_CategoryID");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "categories",
                newName: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_categories_CategoryID",
                table: "transactions",
                column: "CategoryID",
                principalTable: "categories",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
