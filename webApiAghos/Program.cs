
// Program.cs
using Oracle.ManagedDataAccess.Client;
using webApiAghos;
//using webApiAghos.Oracle;
using webApiAghos.Repositories;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

OracleConfiguration.TnsAdmin = configuration.GetSection("ConnectionStrings").GetSection("WalletLocation").Value;
OracleConfiguration.WalletLocation = OracleConfiguration.TnsAdmin;

// Add services to the container.
builder.Services.AddSingleton<IConfiguration>(configuration);
builder.Services.AddTransient<IOracleHelper, OracleHelper>();
builder.Services.AddTransient<ITUserRepository, TUserRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();