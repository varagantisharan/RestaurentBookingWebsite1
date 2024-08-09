
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Entity_Layer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using NuGet.Protocol.Core.Types;
using RestaurentBookingWebsite.DbModels;
using RestaurentBookingWebsite.Services;
using System.Net;
//using Microsoft.AspNet.Identity;


var builder = WebApplication.CreateBuilder(args);


ConfigurationManager configuration = builder.Configuration;

builder.Services.Configure<MailSettings>(configuration.GetSection("MailSettings"));


// Add services to the container.
builder.Services.AddTransient<IMail, RestaurentBookingWebsite.Services.MailServices>();
builder.Services.AddControllersWithViews();
builder.Services.AddMvc();
builder.Services.AddTransient(typeof(RestaurantContext));
builder.Services.AddTransient(typeof(LoginService));
builder.Services.AddTransient(typeof(ILogin), typeof(LoginService));
builder.Services.AddTransient(typeof(BookingServices));
builder.Services.AddTransient(typeof(IBooking), typeof(BookingServices));
builder.Services.AddTransient(typeof(MailServices));
builder.Services.AddTransient(typeof(IMail), typeof(MailServices));
builder.Services.AddTransient(typeof(AdminServices));
builder.Services.AddTransient(typeof(IAdmin), typeof(AdminServices));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(option =>
            {
                option.LoginPath = "/LoginPage/SigninUser";
            });
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseSession();

app.UseRouting();
app.UseSwagger();

app.UseSwaggerUI();

app.UseAuthorization();

CookiePolicyOptions CookiePolicyOptions = new CookiePolicyOptions
{
    MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Lax
};
app.UseCookiePolicy(CookiePolicyOptions);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LoginPage}/{action=SigninUser}/{id?}");

app.Run();
