using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<MockRepository>(); // Or AddScoped<MockRepository>()

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient(); // Register HTTP client factory
builder.Services.AddScoped<ScriptCollector>(); 
builder.Services.AddSingleton<PartialViewRenderer>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
    _ = endpoints.MapRazorPages();
});

app.Run();
