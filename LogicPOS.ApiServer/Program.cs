using System.Text;
using LogicPOS.ApiServer.Authentication;
using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();
if (string.IsNullOrWhiteSpace(jwtSettings.SigningKey) || jwtSettings.SigningKey.Length < 32)
{
    throw new InvalidOperationException("Configure Jwt__SigningKey with at least 32 characters before starting LogicPOS.ApiServer.");
}

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
if (allowedOrigins.Length == 0)
{
    throw new InvalidOperationException("Configure at least one Cors:AllowedOrigins entry before starting LogicPOS.ApiServer.");
}

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
builder.Services.Configure<BootstrapUserSettings>(builder.Configuration.GetSection(BootstrapUserSettings.SectionName));
builder.Services.Configure<SystemInformationResponse>(builder.Configuration.GetSection("SystemInformation"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PosClient", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<SystemVersionService>();
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<CompanyService>();
builder.Services.AddScoped<LicensingService>();
builder.Services.AddScoped<TerminalService>();
builder.Services.AddScoped<ApiSystemInformationService>();
builder.Services.AddScoped<HealthService>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<BootstrapUserSeeder>();
builder.Services.AddScoped<DatabaseInitializer>();
builder.Services.AddSingleton<JwtTokenGenerator>();
builder.Services.AddSingleton<PinHasher>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await databaseInitializer.InitializeAsync();

    var bootstrapUserSeeder = scope.ServiceProvider.GetRequiredService<BootstrapUserSeeder>();
    await bootstrapUserSeeder.SeedAsync();
}

app.UseSerilogRequestLogging();
app.UseCors("PosClient");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
