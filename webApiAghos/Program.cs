
// Program.cs
using Oracle.ManagedDataAccess.Client;
using webApiAghos;
//using webApiAghos.Oracle;
using webApiAghos.Repositories;
using webApiAghos.Security;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

OracleConfiguration.TnsAdmin = configuration.GetSection("ConnectionStrings").GetSection("WalletLocation").Value;
OracleConfiguration.WalletLocation = OracleConfiguration.TnsAdmin;

// Add services to the container.
builder.Services.AddSingleton<IConfiguration>(configuration);
builder.Services.AddTransient<IOracleHelper, OracleHelper>();
builder.Services.AddTransient<ITUserRepository, TUserRepository>();
builder.Services.AddOptions<ApiKeyOptions>()
    .BindConfiguration(ApiKeyOptions.SectionName)
    .Validate(options => options.Keys.Any(key => !string.IsNullOrWhiteSpace(key)),
        "Configure at least one API key in ApiKey:Keys.")
    .ValidateOnStart();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.OpenApiSecurityScheme()
    {
        Name = ApiKeyMiddleware.HeaderName,
        Type = Microsoft.OpenApi.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Informe a API key."
    });
    options.AddSecurityRequirement(document => new()
    {
        [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("ApiKey", document)] = []
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

app.UseMiddleware<ApiKeyMiddleware>();

//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
