﻿using KluczToSukcesDoKariery.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var connectionString = _configuration.GetConnectionString("KluczToSukcesDoKarieryContextConnection") ?? throw new InvalidOperationException("Connection string 'KluczToSukcesDoKarieryContextConnection' not found.");

        services.AddDbContext<KluczToSukcesDoKarieryContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            });
        });
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddTransient<IEmailSender, EmailSender>();

        services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedEmail = true;
            options.SignIn.RequireConfirmedAccount = true;
        })
        .AddEntityFrameworkStores<KluczToSukcesDoKarieryContext>()
        .AddDefaultTokenProviders()
        .AddSignInManager<SignInManager<IdentityUser>>();

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
        });

        services.AddAuthorization();

        services.AddMvc();

        services.AddControllersWithViews();

        services.AddRazorPages();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.Use(async (context, next) =>
        {
            var userManager = context.RequestServices.GetService<UserManager<IdentityUser>>();
            var user = await userManager.FindByEmailAsync("admin@admin.com");

            if (user != null)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = $"{context.Request.Scheme}://{context.Request.Host}/Identity/Account/ResetPassword?userId={Uri.EscapeDataString(user.Id)}&code={Uri.EscapeDataString(token)}";
                logger.LogInformation($"Reset password link: {resetLink}");
            }

            await next();

            if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
            {
                // Loguj nieudane próby logowania
                logger.LogWarning($"404 error. Path: {context.Request.Path}");
            }
        });



        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapRazorPages();
        });
    }

}
