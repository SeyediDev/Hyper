using Hyperyek.Accounting.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddHyperyekAccountingApi();

var app = builder.Build();
app.UseAuthorization();
app.MapControllers();
app.Run();
