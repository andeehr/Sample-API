using Asp.Versioning;
using Sample.Api.Config;
using Sample.Api.Helpers;
using Sample.Api.Middleware;
using Sample.Core.Config;
using Sample.Data;
using Sample.Data.Config;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var config = builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddEnvironmentVariables()
    .BuildAndReplacePlaceholders();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOptionsServices((1, "Sample"));

builder.Services.AddApiVersioning(options =>
{
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});

builder.Services.AddDbContext<DataContext>();
builder.Services.AddAppMappers();
builder.Services.AddAppValidators();
builder.Services.AddAppServices();
builder.Services.AddAppRepositories();

builder.Services.Configure<JwtOptions>(config.GetSection(nameof(JwtOptions)));

var jwtOptions = builder.Configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();

builder.Services.AddApiAuthentication(jwtOptions!);

builder.Services.AddScoped<IJwtTokenWrapper, JwtTokenWrapper>();
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddTransient<JwtSecurityTokenHandler>();
builder.Services.Configure<FilterQueryOptions>(config.GetSection(nameof(FilterQueryOptions)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DefaultModelsExpandDepth(-1);
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    db.Database.EnsureCreated();
}

try
{
    logger.LogInformation("Starting the host");

    app.Run();
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred while trying to run the host");
    throw;
}