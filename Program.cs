using RootedWeb.Models;
using Microsoft.EntityFrameworkCore;

// required for Electron.NET
using ElectronNET.API;
using ElectronNET.API.Entities;

var builder = WebApplication.CreateBuilder(args);

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
    options.UseSqlite("Data Source=RootedDB.db"));

var app = builder.Build();

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
        Show = true,
        AutoHideMenuBar = true
    };
    var window = await Electron.WindowManager.CreateWindowAsync(opts);
    window.OnClosed += () => Electron.App.Quit();
}
