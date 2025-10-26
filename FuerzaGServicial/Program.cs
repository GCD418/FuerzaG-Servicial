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