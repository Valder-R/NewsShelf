using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NewsShelf.UserService.Api.Data;
using NewsShelf.UserService.Api.Models;
using NewsShelf.UserService.Api.Options;
using NewsShelf.UserService.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("UserDb")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IExternalTokenProvider, GoogleTokenProvider>();
builder.Services.AddScoped<IExternalOAuthService, ExternalOAuthService>();
builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();

// ======================
// OAuth safe toggle (Docker-friendly)
// ======================
var oauthEnabled = builder.Configuration.GetValue<bool>("OAuth:Enabled");

var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

// Auto-disable OAuth if secrets are missing (avoid crash on startup)
if (oauthEnabled && (string.IsNullOrWhiteSpace(googleClientId) || string.IsNullOrWhiteSpace(googleClientSecret)))
{
    oauthEnabled = false;
    Console.WriteLine("⚠️ Google OAuth disabled: missing Authentication:Google:ClientId or ClientSecret");
}

// ======================
// Auth
// ======================
var auth = builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
            NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier
        };
    });

if (oauthEnabled)
{
    auth.AddGoogle(options =>
    {
        builder.Configuration.GetSection("Authentication:Google").Bind(options);
    });
}

builder.Services.AddAuthorization();

// ======================
// CORS
// ======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NewsShelf User Service",
        Version = "v1",
        Description = "User/account microservice for the NewsShelf platform"
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Input JWT as: Bearer {token}"
    };
    options.AddSecurityDefinition("Bearer", securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

// ======================
// Ensure SQLite directory exists (from ConnectionStrings:UserDb)
// ======================
var connectionString = app.Configuration.GetConnectionString("UserDb");
if (!string.IsNullOrEmpty(connectionString))
{
    var dataSourceIndex = connectionString.IndexOf("Data Source=", StringComparison.OrdinalIgnoreCase);
    if (dataSourceIndex >= 0)
    {
        var dbPath = connectionString.Substring(dataSourceIndex + "Data Source=".Length).Trim();

        var semicolonIndex = dbPath.IndexOf(';');
        if (semicolonIndex >= 0)
        {
            dbPath = dbPath.Substring(0, semicolonIndex).Trim();
        }

        if (!Path.IsPathRooted(dbPath))
        {
            dbPath = Path.Combine(app.Environment.ContentRootPath, dbPath);
        }

        var dbDirectory = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(dbDirectory) && !Directory.Exists(dbDirectory))
        {
            Directory.CreateDirectory(dbDirectory);
        }
    }
}

// Extra folder if you want it (safe)
var dataDirectory = Path.Combine(app.Environment.ContentRootPath, "Data");
Directory.CreateDirectory(dataDirectory);

// ======================
// Migrations + seed roles/admin
// ======================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "ADMIN", "MODERATOR", "READER" };
    foreach (var roleName in roles)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    var adminEmail = app.Configuration["Admin:Email"] ?? "admin@newsshelf.com";
    var adminPassword = app.Configuration["Admin:Password"] ?? "Admin123!";
    var adminDisplayName = app.Configuration["Admin:DisplayName"] ?? "System Administrator";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            DisplayName = adminDisplayName
        };

        var createResult = await userManager.CreateAsync(adminUser, adminPassword);
        if (createResult.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "ADMIN");
        }
    }
    else
    {
        if (!await userManager.IsInRoleAsync(adminUser, "ADMIN"))
        {
            await userManager.AddToRoleAsync(adminUser, "ADMIN");
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
