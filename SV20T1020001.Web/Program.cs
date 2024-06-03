using Microsoft.AspNetCore.Authentication.Cookies;
using SV20T1020001.Web;

var builder = WebApplication.CreateBuilder(args);
//Add các service cần dùng trong Application
builder.Services.AddHttpContextAccessor();
// Add services to the container.
builder.Services.AddControllersWithViews()
     .AddMvcOptions(option => {
         //không dùng thông báo lỗi mặc định
         option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
     });
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(60);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(option =>
				{
					option.Cookie.Name = "AuthenticationCookie";
					option.LoginPath = "/Account/Login";
					option.AccessDeniedPath = "/Account/AccessDenined";
					option.ExpireTimeSpan = TimeSpan.FromMinutes(60);
				});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();//kiem tra xem co the truy cap khong
app.UseAuthorization();//cap quyen su dung

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

ApplicationContext.Configure
(
    httpContextAccessor: app.Services.GetRequiredService<IHttpContextAccessor>(),
    hostEnvironment: app.Services.GetService<IWebHostEnvironment>()
);

string connectionString = "server=.;user id=sa;password=123456;database=LiteCommerceDB; TrustServerCertificate=true";
SV20T1020001.BusinessLayers.Configuration.Initialize(connectionString);

app.Run();
