using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using VitalPhotography.Api.Data;
using VitalPhotography.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Database — SQLite everywhere; file path set via connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=vital-photography.db";

builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite(connectionString));

// JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("Jwt:Secret is required");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ImageService>();

// Storage — local filesystem (photos stored on the Render persistent disk)
builder.Services.AddScoped<IStorageService, LocalStorageService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o =>
    o.AddDefaultPolicy(p => p
        .WithOrigins(builder.Configuration["Cors:AllowedOrigins"]?.Split(',') ?? ["http://localhost:3000"])
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

// Auto-migrate on startup (dev); use explicit migration step in prod CI
using (var scope = app.Services.CreateScope())
    await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Serve uploaded photos — from /data/uploads in prod (Render disk) or wwwroot/uploads in dev
var uploadsPath = app.Configuration["Storage:Root"]
    ?? Path.Combine(builder.Environment.WebRootPath, "uploads");
Directory.CreateDirectory(uploadsPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider    = new PhysicalFileProvider(uploadsPath),
    RequestPath     = "/uploads",
    ContentTypeProvider = new FileExtensionContentTypeProvider
    {
        Mappings = { [".webp"] = "image/webp" }
    },
    OnPrepareResponse = ctx =>
        ctx.Context.Response.Headers[HeaderNames.CacheControl] =
            "public, max-age=31536000, immutable",
});

// Short cache on API list responses (photos can be toggled visible/invisible)
app.Use(async (ctx, next) =>
{
    await next();
    if (ctx.Request.Path.StartsWithSegments("/api/v1/photos") && ctx.Response.StatusCode == 200)
        ctx.Response.Headers[HeaderNames.CacheControl] = "public, max-age=60, stale-while-revalidate=300";
});

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
