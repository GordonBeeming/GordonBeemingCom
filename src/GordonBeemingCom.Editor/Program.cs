using System.Globalization;
using GordonBeemingCom.Database.Tables;
using GordonBeemingCom.Editor.Areas.Identity;
using GordonBeemingCom.Editor.Data;
using GordonBeemingCom.Shared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Debug;

var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
var cultureNumberFormat = (NumberFormatInfo)culture.NumberFormat.Clone();
cultureNumberFormat.CurrencySymbol = "$";
cultureNumberFormat.CurrencyDecimalSeparator = ".";
cultureNumberFormat.NumberDecimalSeparator = ".";
culture.NumberFormat = cultureNumberFormat;
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

#if DEBUG
var debugLoggerFactory = new LoggerFactory(new[] { new DebugLoggerProvider() });
#endif

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
  options.UseSqlServer(connectionString, sqlServerOptions => sqlServerOptions.CommandTimeout(300));
#if DEBUG
  options.UseLoggerFactory(debugLoggerFactory);
  options.EnableSensitiveDataLogging(true);
#endif
});
builder.Services.AddDbContext<AppDbContext>(options =>
{
  options.UseSqlServer(connectionString, sqlServerOptions => sqlServerOptions.CommandTimeout(300));
#if DEBUG
  options.UseLoggerFactory(debugLoggerFactory);
  options.EnableSensitiveDataLogging(true);
#endif
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
  {
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;
  })
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
  options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
  options.KnownNetworks.Clear();
  options.KnownProxies.Clear();
});

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddSingleton<IBlobServiceClientService, BlobServiceClientService>();
builder.Services.AddSingleton<HashHelper>();
builder.Services.AddSingleton<DeploymentInfo>();
builder.Services.AddScoped<IExternalUrlsService, ExternalUrlsService>();

var app = builder.Build();

var serviceScopeFactory = app.Services.GetService<IServiceScopeFactory>();
using (var scope = serviceScopeFactory!.CreateScope())
{
  var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
  appDbContext.Database.Migrate();
  if (appDbContext.Categories.FirstOrDefault() == null)
  {
    appDbContext.Categories.Add(new Categories() { Id = Guid.NewGuid(), CategoryName = "Developer", CategorySlug = "developer", DisplayIndex = 1, HexColour = "00FF00",});
    appDbContext.SaveChanges();

  }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseMigrationsEndPoint();
}
else
{
  app.UseExceptionHandler("/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
