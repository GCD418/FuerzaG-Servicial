using CommonService.Domain.Ports;
using CommonService.Domain.Services.Validations;
using CommonService.Infrastructure;
using CommonService.Infrastructure.Connection;
using FuerzaGServicial.Infrastructure.Security;
using OwnerService.Domain.Entities;
using OwnerService.Domain.Ports;
using OwnerService.Domain.Services;
using OwnerService.Infrastructure.Persistence;
using UserAccountService.Domain.Entities;
using UserAccountService.Domain.Ports;
using UserAccountService.Domain.Services;
using UserAccountService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

#region AuthenticationAndPasswords

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
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<ISessionManager, CurrentUserSession>();

#endregion

#region DatabaseConnection

var connectionString = builder.Configuration.GetConnectionString("PostgreSql");
var connectionManager = DatabaseConnectionManager.GetInstance(connectionString!);
builder.Services.AddSingleton(connectionManager);
builder.Services.AddScoped<IDbConnectionFactory, PostgreSqlConnection>();

#endregion

#region Owner

builder.Services.AddScoped<OwnerService.Application.Services.OwnerService>();
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped<IValidator<Owner>,  OwnerValidator>();

#endregion

#region UserAccount

builder.Services.AddScoped<UserAccountService.Application.Services.UserAccountService>();
builder.Services.AddScoped<IUserAccountRepository, UserAccountRepository>();
builder.Services.AddScoped<IValidator<UserAccount>, UserAccountValidator>();

#endregion

#region MailSettings

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IMailSender, SmtpEmailSender>();

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

//For session
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

app.Run();