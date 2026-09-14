using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hyper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalOAuthToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExternalOAuthTokens",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConnectionId = table.Column<long>(type: "bigint", nullable: false),
                    ShopId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TokenType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Scopes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssuedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    RawTokenResponse = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalOAuthTokens", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalOAuthTokens_ConnectionId",
                schema: "dbo",
                table: "ExternalOAuthTokens",
                column: "ConnectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalOAuthTokens_ExpiresAtUtc",
                schema: "dbo",
                table: "ExternalOAuthTokens",
                column: "ExpiresAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalOAuthTokens_TenantId",
                schema: "dbo",
                table: "ExternalOAuthTokens",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "UX_ExternalOAuthTokens_ShopId_Provider",
                schema: "dbo",
                table: "ExternalOAuthTokens",
                columns: new[] { "ShopId", "Provider" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalOAuthTokens",
                schema: "dbo");
        }
    }
}
