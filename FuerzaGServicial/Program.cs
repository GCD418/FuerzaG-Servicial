using CommonService.Infrastructure.Connection;
using OwnerService.Domain.Ports;
using OwnerService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

//Authentication management
builder.Services
    .AddAuthentication("GForceAuth")
    .AddCookie("GForceAuth", options =>
    {
        options.Cookie.Name = "GForceCookie";
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/AccessDenied"; //TODO
        options.LogoutPath = "/Logout"; //TODO
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

#region DatabaseConnection

var connectionString = builder.Configuration.GetConnectionString("PostgreSql");
var connectionManager = DatabaseConnectionManager.GetInstance(connectionString!);
builder.Services.AddSingleton(connectionManager);
builder.Services.AddScoped<IDbConnectionFactory, PostgreSqlConnection>();

#endregion

#region Owner

builder.Services.AddScoped<OwnerService.Application.Services.OwnerService>();
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();

#endregion

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

app.Run();