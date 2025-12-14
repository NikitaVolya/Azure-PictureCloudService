using Microsoft.EntityFrameworkCore;
using PictureCloudService.Data;
using PictureCloudService.Services;

var key = "This_is_my_first_Test_Key_That_Is_Long_Enough_123!";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

string connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") ?? throw new InvalidOperationException("DATABASE_CONNECTION_STRING environment variable is not set.");
builder.Services.AddDbContext<AppDbContext>(options =>  options.UseSqlServer(connectionString));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UserService>();


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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
