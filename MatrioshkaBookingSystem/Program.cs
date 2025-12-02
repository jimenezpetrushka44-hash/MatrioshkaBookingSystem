using Microsoft.EntityFrameworkCore;
using MatrioshkaBookingSystem.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("BookingConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("BookingConnection"))
    )
);

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
