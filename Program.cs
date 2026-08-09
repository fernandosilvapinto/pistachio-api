using Pistachio.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Pistachio.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuração do banco de dados.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// CORS - restrito para o frontend em dev
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", p => p
        .WithOrigins("http://localhost:5173")
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
builder.Services.AddSwaggerGen();

// JWT Auth
var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["Key"] ?? throw new InvalidOperationException("JWT Key not configured."));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

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

app.MapControllers();

app.Run();
