using Neo.Domain.Features.Telementry;
using Neo.Infrastructure.Features.Telementry;

var builder = WebApplication.CreateBuilder(args);

// Configure Telemetry
builder.Services.Configure<TelemetryOptions>(builder.Configuration.GetSection(nameof(TelemetryOptions)));
builder.Host.AddNeoSerilog();
builder.Services.AddNeoOpenTelementry(builder.Configuration);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
