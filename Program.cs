using FirstCoreWebApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔹 DB Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Add Controllers
builder.Services.AddControllers();

// add authentication by using Bearer Manner
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = builder.Configuration["Jwt:Key"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key)),

            ClockSkew = TimeSpan.Zero
        };
    });
// 🔹 Swagger (JWT support ke saath)
builder.Services.AddEndpointsApiExplorer();

// Swagger configure and \ Enable jwt authentication 
builder.Services.AddSwaggerGen(options =>
{
    // their is securit type in api that is [Bearer]
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization", // Name of the heder
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http, //http based authentication
        Scheme = "bearer", 
        BearerFormat = "JWT",  // swgger tells UI that this is a JWT Token
        In = Microsoft.OpenApi.Models.ParameterLocation.Header, //define location of token i.e in header or in body
        Description = "Enter token like: Bearer {your token}" // 
    });
    // For API security
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// 🔹 Middleware pipeline

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🔴 IMPORTANT ORDER
app.UseAuthentication();   // ✅ JWT check yaha hota hai
app.UseAuthorization();    // ✅ role/permission check

app.MapControllers();

app.Run();