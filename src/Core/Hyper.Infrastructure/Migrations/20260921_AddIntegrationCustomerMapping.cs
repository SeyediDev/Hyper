using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Hyper.Infrastructure.Data.Repository.Hyper;

#nullable disable
namespace Hyper.Infrastructure.Migrations;

[DbContext(typeof(HyperSqlServerContext))]
[Migration("20260921_AddIntegrationCustomerMapping")]
public partial class AddIntegrationCustomerMapping : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(name: "IntegrationCustomerMappings", schema: "dbo",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                ShopId = table.Column<int>(type: "int", nullable: false),
                TenantId = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                BasalamUserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                PersonId = table.Column<int>(type: "int", nullable: false)
            }, constraints: table => table.PrimaryKey("PK_IntegrationCustomerMappings", x => x.Id));
        migrationBuilder.CreateIndex(name: "UX_IntegrationCustomerMappings_ExternalIdentity", schema: "dbo",
            table: "IntegrationCustomerMappings", columns: new[] { "ShopId", "TenantId", "BasalamUserId" }, unique: true);
        migrationBuilder.CreateIndex(name: "IX_IntegrationCustomerMappings_Person", schema: "dbo",
            table: "IntegrationCustomerMappings", columns: new[] { "ShopId", "TenantId", "PersonId" }, unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable(name: "IntegrationCustomerMappings", schema: "dbo");
}
