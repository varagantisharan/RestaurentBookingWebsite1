using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using RestaurentBookingWebsite.DbModels;
using RestaurentBookingWebsite.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient<IMail,MailServices>();
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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
