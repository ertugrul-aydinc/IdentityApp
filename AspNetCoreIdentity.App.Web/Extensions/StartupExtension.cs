using AspNetCoreIdentity.App.Web.CustomValidations;
using AspNetCoreIdentity.App.Web.Localization;
using AspNetCoreIdentity.App.Repository.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentity.App.Web.Extensions
{
    public static class StartupExtension
    {
        public static void AddIdentityWithExtension(this IServiceCollection services)
        {
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(2);
            });


            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvyz1234567890_";

                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = true;

            }).AddPasswordValidator<PasswordValidator>()
              .AddUserValidator<UserValidator>()
              //.AddErrorDescriber<LocalizationIdentityErrorDescriber>()
              .AddDefaultTokenProviders()
              .AddEntityFrameworkStores<AppDbContext>();
        }
    }
}