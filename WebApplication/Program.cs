using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

//var builder = WebApplication.CreateBuilder(args);
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;


//var builder = WebApplication.CreateBuilder(args);
using RestaurentBookingWebsite.Services;
//var builder = WebApplication.CreateBuilder(args);
using RestaurentBookingWebsite.Services;
using System.Net;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//builder.Services.AddTransient(typeof(LoginService));
//builder.Services.AddTransient(typeof(ILogin), typeof(LoginService));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(option =>
            {

                option.LoginPath = "/Login/SigninUser";

            });




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthentication();
app.UseStatusCodePages(context =>
{
    var response = context.HttpContext.Response;
    if (response.StatusCode == (int)HttpStatusCode.Unauthorized ||
        response.StatusCode == (int)HttpStatusCode.Forbidden)
        response.Redirect("/Login/Login");
    return Task.CompletedTask;
});

app.UseAuthorization();
CookiePolicyOptions CookiePolicyOptions = new CookiePolicyOptions
{
    MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Lax
};
app.UseCookiePolicy(CookiePolicyOptions);



app.MapControllers();

app.Run();
