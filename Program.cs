using Hangfire;
using Microsoft.EntityFrameworkCore;
using WebApp.Common;
using WebApp.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>( options => { 
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLDbConnection"));
});
builder.Services.AddHangfire((sp, config) =>
{
    config.UseSqlServerStorage(sp.GetRequiredService<IConfiguration>().GetConnectionString("SQLDbConnection"));
});
builder.Services.AddHangfireServer();
builder.Services.AddLogging();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseHangfireDashboard("/jobs/dashboard", new DashboardOptions
{
    DashboardTitle = "Jobs Dashboard",
    DarkModeEnabled = true,
    DisplayStorageConnectionString = false
});
app.Run();
