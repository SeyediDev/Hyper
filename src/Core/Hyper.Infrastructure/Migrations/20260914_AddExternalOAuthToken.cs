using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hyper.Infrastructure.Migrations;

/// <summary>Historical no-op: OAuth schema is owned by HyperIntegrationContext.</summary>
public partial class AddExternalOAuthToken : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) { }
    protected override void Down(MigrationBuilder migrationBuilder) { }
}
