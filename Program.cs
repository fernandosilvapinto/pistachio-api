using Pistachio.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pistachio.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Pistachio.Api.Authorization;
using Pistachio.Api.Identity;

var builder = WebApplication.CreateBuilder(args);

// Configuração do banco de dados.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// CORS - restrito para o frontend em dev
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", p => p
        .WithOrigins("http://localhost:5173", "http://localhost:5174")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

// Controllers e JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.WriteIndented = true; // opcional: deixa o JSON formatado
            options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        });
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insere o token JWT (sem o prefixo 'Bearer ', o Swagger adiciona automaticamente)"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// JWT Auth
var audience = builder.Configuration["Keeper:Audience"]
    ?? throw new InvalidOperationException("Keeper:Audience is not configured.");
var workforceAuthority = builder.Configuration["Keeper:Workforce:Authority"]
    ?? throw new InvalidOperationException("Keeper:Workforce:Authority is not configured.");
var customersAuthority = builder.Configuration["Keeper:Customers:Authority"]
    ?? throw new InvalidOperationException("Keeper:Customers:Authority is not configured.");

static Action<JwtBearerOptions> Realm(string authority, string audience) => options =>
{
    options.Authority = authority;
    options.Audience = audience;
    options.RequireHttpsMetadata = false;
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.FromSeconds(30),
        NameClaimType = "preferred_username"
    };
};

builder.Services
    .AddAuthentication(IssuerSchemes.Selector)
    .AddPolicyScheme(IssuerSchemes.Selector, IssuerSchemes.Selector, options =>
    {
        options.ForwardDefaultSelector = context => IssuerSchemes.Select(context, customersAuthority);
    })
    .AddJwtBearer(IssuerSchemes.Workforce, Realm(workforceAuthority, audience))
    .AddJwtBearer(IssuerSchemes.Customers, Realm(customersAuthority, audience));

builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddScoped<UserProvisioning>();

builder.Services.Configure<KeeperAdminOptions>(builder.Configuration.GetSection("Keeper:Admin"));
builder.Services.AddHttpClient<KeeperAdminClient>();

builder.Services.AddAuthorization(options =>
{
    foreach (var permission in Permissions.All)
    {
        options.AddPolicy(permission, policy => policy.Requirements.Add(new PermissionRequirement(permission)));
    }
});

// Envio de email (Mailpit em dev)
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();

        logger.LogInformation("Applying database migrations...");

        dbContext.Database.Migrate();

        await SeedData.InitializeAsync(dbContext);

        logger.LogInformation("Database migrations completed.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Migration failed.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("DevCors");

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<UserProvisioningMiddleware>();

app.MapControllers();

app.Run();
