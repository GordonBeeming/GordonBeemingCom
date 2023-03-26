using System.Configuration;
using System.Globalization;
using GordonBeemingCom.Data;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Net.Http.Headers;

var culture = (CultureInfo)CultureInfo.GetCultureInfo("en-us").Clone();
var cultureNumberFormat = (NumberFormatInfo)culture.NumberFormat.Clone();
cultureNumberFormat.CurrencySymbol = "R";
cultureNumberFormat.CurrencyDecimalSeparator = ".";
cultureNumberFormat.NumberDecimalSeparator = ".";
culture.NumberFormat = cultureNumberFormat;
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

#if DEBUG
var debugLoggerFactory = new LoggerFactory(new[] { new DebugLoggerProvider() });
#endif

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

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
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
  options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
  options.KnownNetworks.Clear();
  options.KnownProxies.Clear();
});

builder.Services.AddResponseCaching();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllersWithViews();
builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddTransient<IBlobServiceClientService, BlobServiceClientService>();
builder.Services.AddTransient<IFileService, FileService>();

builder.Services.Configure<SiteConfig>(builder.Configuration.GetSection("SiteConfig"));

builder.Services.AddSingleton<HashHelper>();

var app = builder.Build();

//app.UseHttpsRedirection();

// Configure the HTTP request pipeline. 
if (app.Environment.IsDevelopment())
{
  builder.Services.AddDatabaseDeveloperPageExceptionFilter();

  app.UseDeveloperExceptionPage();

  app.UseMigrationsEndPoint();

  app.UseStaticFiles(new StaticFileOptions
  {
  });
}
else
{
  app.UseExceptionHandler("/status-code");
  
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  //app.UseHsts();


  app.UseResponseCaching();
  app.UseStaticFiles(new StaticFileOptions
  {
    OnPrepareResponse = ctx =>
    {
      const int durationInSeconds = 60 * 60 * 24 * 30;
      ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=" + durationInSeconds;
    }
  });
}
app.UseStatusCodePagesWithReExecute("/status-code", "?statusCode={0}");

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
  );
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
