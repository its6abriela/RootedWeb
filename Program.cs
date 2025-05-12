using System;
using System.IO;
using RootedWeb.Models;
using Microsoft.EntityFrameworkCore;

// required for Electron.NET
using ElectronNET.API;
using ElectronNET.API.Entities;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);

var dbFile = Path.Combine(AppContext.BaseDirectory, "RootedDB.db");

//  Register Electron.NET
builder.WebHost.UseElectron(args);
builder.Services.AddElectron();

//  Your existing registrations
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<RootedContext>(options =>
    options.UseSqlite($"Data Source={dbFile}"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<RootedContext>();
    ctx.Database.EnsureCreated();    // will create any missing tables in that file
}

app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//  **Before** app.Run(), launch your Electron window if we're in Electron
if (HybridSupport.IsElectronActive)
{
    _ = CreateElectronWindowAsync();
}

app.Run();


//  Your window-creation helper
static async Task CreateElectronWindowAsync()
{
    var opts = new BrowserWindowOptions
    {
        Width = 1200,
        Height = 800,
        Show = false,
        AutoHideMenuBar = true,
        Icon = Path.Combine(AppContext.BaseDirectory, "build", "icons", "app.ico")
    };
    var window = await Electron.WindowManager.CreateWindowAsync(opts);
    window.Maximize();
    window.Show();
    window.OnClosed += () => Electron.App.Quit();
}
