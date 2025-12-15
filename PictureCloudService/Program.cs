using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PictureCloudService.Data;
using PictureCloudService.Profiles;
using PictureCloudService.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

string connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") ?? throw new InvalidOperationException("DATABASE_CONNECTION_STRING environment variable is not set.");
builder.Services.AddDbContext<AppDbContext>(options =>  options.UseSqlServer(connectionString));


builder.Services.AddScoped<PictureComputerVisionService>();
builder.Services.AddScoped<PictureBlobStorage>();
builder.Services.AddScoped<PictureService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UserService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";
        options.AccessDeniedPath = "/Home/Index";
    });

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
