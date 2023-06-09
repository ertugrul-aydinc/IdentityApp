using AspNetCoreIdentity.App.Web.ClaimProvider;
using AspNetCoreIdentity.App.Web.Extensions;
using AspNetCoreIdentity.App.Repository.Models;
using AspNetCoreIdentity.App.Core.OptionsModel;
using AspNetCoreIdentity.App.Web.Requirements;
using AspNetCoreIdentity.App.Web.Seeds;
using AspNetCoreIdentity.App.Service.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using AspNetCoreIdentity.App.Core.PermissionsRoot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"), options =>
    {
        options.MigrationsAssembly("AspNetCoreIdentity.App.Repository");
    });
});

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.FromMinutes(30);
});


builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));


builder.Services.Configure<EmailInfos>(builder.Configuration.GetSection("MailInfos"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IClaimsTransformation, UserClaimProvider>();
builder.Services.AddScoped<IAuthorizationHandler, ExchangeExpireRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ViolenceRequirementHandler>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IHomeService, HomeService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AnkaraPolicy", policy =>
    {
        policy.RequireClaim("city", "ankara");
    });

    options.AddPolicy("ExchangePolicy", policy =>
    {
        policy.AddRequirements(new ExchangeExpireRequirement());
    });

    options.AddPolicy("ViolancePolicy", policy =>
    {
        policy.AddRequirements(new ViolenceRequirement() { ThresholdAge = 18});
    });

    options.AddPolicy("OrderPermissionReadAndDelete", policy =>
    {
        policy.RequireClaim("permission", Permissions.Order.Read);
        policy.RequireClaim("permission", Permissions.Order.Delete);
        policy.RequireClaim("permission", Permissions.Stock.Delete);
    });

    options.AddPolicy("Permission.Order.Read", policy =>
    {
        policy.RequireClaim("permission", Permissions.Order.Read);
    });

    options.AddPolicy("Permission.Order.Delete", policy =>
    {
        policy.RequireClaim("permission", Permissions.Order.Delete);
    });

    options.AddPolicy("Permission.Stock.Delete", policy =>
    {
        policy.RequireClaim("permission", Permissions.Stock.Delete);
    });
});

builder.Services.AddIdentityWithExtension();

builder.Services.ConfigureApplicationCookie(options =>
{
    var cookieBuilder = new CookieBuilder();
    cookieBuilder.Name = "UdemyAppCookie";
    cookieBuilder.SameSite = SameSiteMode.Lax; //3.taraflardan kaynaklanan isteklere cookie biligisinin gönderilme ayarý
    cookieBuilder.SecurePolicy = CookieSecurePolicy.Always;//Cookileri https üzerinden eriþilebilir yapar

    options.LoginPath = new PathString("/Home/SignIn");
    options.LogoutPath = new PathString("/Members/LogOut");
    options.AccessDeniedPath = new PathString("/Members/AccessDenied");
    options.Cookie = cookieBuilder;
    options.ExpireTimeSpan = TimeSpan.FromDays(60);
    options.SlidingExpiration = true; // Her giriþ yaptýðýnda süre 60 gün uzar
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

    await PermissionSeed.Seed(roleManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
