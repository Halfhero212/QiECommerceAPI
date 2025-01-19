using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QiECommerceAPI.Data;
using QiECommerceAPI.Middleware;
using QiECommerceAPI.Repositories;
using QiECommerceAPI.Services;
using System.Text;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure EF Core DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Configure JWT auth
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "qi.com",
            ValidAudience = "qi-users",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("YourStrongSecretKey")
            ),
    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        };

        // Add these events to see detailed info in your console
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("JWT Authentication Failed: " + context.Exception.Message);
                return Task.CompletedTask;
            },
 OnTokenValidated = context =>
    {
        // Show the raw token the server actually received
        var token = context.SecurityToken as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
        Console.WriteLine("Server sees token: " + (token?.RawData ?? "null"));

        var claims = context.Principal?.Claims.Select(c => c.Type + "=" + c.Value) ?? new string[0];
        Console.WriteLine("Claims: " + string.Join(", ", claims));
        return Task.CompletedTask;
    }
};
    });


// 3. Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
});

// 4. Register Repositories & Services (DI)
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// 5. Add Controllers + FluentValidation
builder.Services.AddControllers()
    .AddFluentValidation(config =>
    {
        config.RegisterValidatorsFromAssemblyContaining<Program>(); 
    });

// 6. Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 7. Build the app
var app = builder.Build();

// 8. Migrate database on startup (optional)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// 9. Use global exception middleware
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// 10. Configure middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
