using Hyperyek.Accounting.Api;
using Hyperyek.Accounting.Infrastructure;
using Hyperyek.Accounting.Host;

var builder = WebApplication.CreateBuilder(args);
var connection = builder.Configuration.GetConnectionString("Domain")
    ?? builder.Configuration.GetConnectionString("DomainCommandConnection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Accounting database connection is required.");
builder.Services.AddControllers().AddApplicationPart(typeof(AccountingCommandsController).Assembly);
builder.Services.AddAccountingServiceSecurity(builder.Configuration);
builder.Services.AddHyperyekAccountingApi();
builder.Services.AddHyperyekSqlAccounting(connection);

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
