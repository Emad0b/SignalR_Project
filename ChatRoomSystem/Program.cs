using Microsoft.EntityFrameworkCore;
using ChatRoomSystem.Hubs;
using ChatRoomSystem.Models; 

var builder = WebApplication.CreateBuilder(args);

// Configure Entity Framework Core
builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection")));

// Register SignalR service
builder.Services.AddSignalR();

// Add MVC controllers with views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Map the default MVC route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map SignalR hub
app.MapHub<ChatHub>("/chatHub");

app.Run();
