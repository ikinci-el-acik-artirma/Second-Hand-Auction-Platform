using AuctionSystem.API.Data;
using AuctionSystem.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("AuctionSystemDb"));

builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<AuctionItemService>();

builder.Services.AddHttpClient<DevicePhotoService>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
