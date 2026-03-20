using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayPing.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPaidColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "PaymentHistories",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "PaymentHistories");
        }
    }
}
