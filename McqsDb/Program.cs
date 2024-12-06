using Mcq.Models;
using Microsoft.EntityFrameworkCore;
using Mcq.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<McqsDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddDistributedMemoryCache();  // Sử dụng bộ nhớ để lưu trữ session (hoặc sử dụng Redis, SQL nếu cần)

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(1000);  // Thời gian hết hạn session sau 30 giây không hoạt động
    options.Cookie.HttpOnly = true;  // Chỉ có thể truy cập session qua HTTP, không qua JavaScript
    options.Cookie.IsEssential = true;  // Cần thiết cho ứng dụng (cho phép lưu trữ session)
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthorization();
app.UseMiddleware<AuthenticationMiddleware>();
app.UseMiddleware<AuthorizationMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
