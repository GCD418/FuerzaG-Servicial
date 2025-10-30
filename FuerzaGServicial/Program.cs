using CommonService.Domain.Entities;
using CommonService.Domain.Ports;
using CommonService.Domain.Services;
using CommonService.Domain.Services.Validations;
using CommonService.Infrastructure;
using CommonService.Infrastructure.Connection;
using CommonService.Infrastructure.Reports;
using CommonService.Infrastructure.Reports.Repositories;
using FuerzaGServicial.Application.Services;
using FuerzaGServicial.Infrastructure.Security;
using OwnerService.Domain.Entities;
using OwnerService.Domain.Ports;
using OwnerService.Domain.Services;
using OwnerService.Infrastructure.Persistence;
using TechnicianService.Domain.Entities;
using TechnicianService.Domain.Ports;
using TechnicianService.Domain.Services;
using TechnicianService.Infrastructure.Persistence;
using UserAccountService.Application.Facades;
using UserAccountService.Domain.Entities;
using UserAccountService.Domain.Ports;
using UserAccountService.Domain.Services;
using UserAccountService.Infrastructure.Persistence;
using ServiceService.Domain.Ports;
using ServiceService.Infrastructure.Persistence;   
using ServiceService.Domain.Entities;
using ServiceService.Domain.Services;

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
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<ISessionManager, CurrentUserSession>();
builder.Services.AddScoped<SessionFacade>();
builder.Services.AddScoped<IValidator<CommonService.Domain.Entities.ChangePasswordInput>, ChangePasswordInputValidator>();

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


#region Technician

builder.Services.AddScoped<TechnicianService.Application.Services.TechnicianService>();
builder.Services.AddScoped<ITechnicianRepository, TechnicianRepository>();
builder.Services.AddScoped<IValidator<Technician>, TechnicianValidator>();

#endregion

#region Service
builder.Services.AddScoped<ServiceService.Application.Services.ServiceService>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IValidator<Service>, ServiceValidator>();

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

#region Reports

builder.Services.AddScoped<CommonService.Domain.Services.ReportDirector>();
builder.Services.AddScoped<CommonService.Domain.Ports.IChartGenerator, CommonService.Infrastructure.Reports.ChartGenerator>();
builder.Services.AddScoped<CommonService.Domain.Ports.IVehicleReportRepository, CommonService.Infrastructure.Reports.Repositories.VehicleReportRepository>();
builder.Services.AddScoped<CommonService.Domain.Ports.IBrandReportRepository, CommonService.Infrastructure.Reports.Repositories.BrandReportRepository>();
builder.Services.AddScoped<FuerzaGServicial.Application.Services.VehicleReportService>();
builder.Services.AddScoped<FuerzaGServicial.Application.Services.BrandReportService>();

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