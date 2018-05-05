/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.DependencyInjection;
using Piranha.AspNetCore.Identity;
using Piranha.AspNetCore.Identity.Data;
using System;
using System.Reflection;

public static class IdentityModuleExtensions
{
    /// <summary>
    /// Adds the Piranha identity module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddPiranhaIdentity(this IServiceCollection services, 
        Action<DbContextOptionsBuilder> dbOptions, 
        Action<IdentityOptions> identityOptions = null,
        Action<CookieAuthenticationOptions> cookieOptions = null) 
    {
        // Add the identity module
        Piranha.App.Modules.Register<Piranha.AspNetCore.Identity.Module>();

        // Register views
        var assembly = typeof(IdentityModuleExtensions).GetTypeInfo().Assembly;
        var provider = new EmbeddedFileProvider(assembly, "Piranha.AspNetCore.Identity");

        // Add the file provider to the Razor view engine
        services.Configure<RazorViewEngineOptions>(options =>
        {
            options.FileProviders.Add(provider);
        });

        // Setup authorization policies
        services.AddAuthorization(o => {
            // Role policies
            o.AddPolicy(Permissions.Roles, policy => {
                policy.RequireClaim(Piranha.Manager.Permission.Admin, Piranha.Manager.Permission.Admin);
                policy.RequireClaim(Permissions.Roles, Permissions.Roles);
            });
            o.AddPolicy(Permissions.RolesAdd, policy => {
                policy.RequireClaim(Piranha.Manager.Permission.Admin, Piranha.Manager.Permission.Admin);
                policy.RequireClaim(Permissions.Roles, Permissions.Roles);
                policy.RequireClaim(Permissions.RolesAdd, Permissions.RolesAdd);
            });
            o.AddPolicy(Permissions.RolesDelete, policy => {
                policy.RequireClaim(Piranha.Manager.Permission.Admin, Piranha.Manager.Permission.Admin);
                policy.RequireClaim(Permissions.Roles, Permissions.Roles);
                policy.RequireClaim(Permissions.RolesDelete, Permissions.RolesDelete);
            });
            o.AddPolicy(Permissions.RolesEdit, policy => {
                policy.RequireClaim(Piranha.Manager.Permission.Admin, Piranha.Manager.Permission.Admin);
                policy.RequireClaim(Permissions.Roles, Permissions.Roles);
                policy.RequireClaim(Permissions.RolesEdit, Permissions.RolesEdit);
            });
            o.AddPolicy(Permissions.RolesSave, policy => {
                policy.RequireClaim(Piranha.Manager.Permission.Admin, Piranha.Manager.Permission.Admin);
                policy.RequireClaim(Permissions.Roles, Permissions.Roles);
                policy.RequireClaim(Permissions.RolesSave, Permissions.RolesSave);
            });

            // User policies
            o.AddPolicy(Permissions.Users, policy => {
                policy.RequireClaim(Piranha.Manager.Permission.Admin, Piranha.Manager.Permission.Admin);
                policy.RequireClaim(Permissions.Users, Permissions.Users);
            });
            o.AddPolicy(Permissions.UsersAdd, policy => {
                policy.RequireClaim(Piranha.Manager.Permission.Admin, Piranha.Manager.Permission.Admin);
                policy.RequireClaim(Permissions.Users, Permissions.Users);
                policy.RequireClaim(Permissions.UsersAdd, Permissions.UsersAdd);
            });
            o.AddPolicy(Permissions.UsersDelete, policy => {
                policy.RequireClaim(Piranha.Manager.Permission.Admin, Piranha.Manager.Permission.Admin);
                policy.RequireClaim(Permissions.Users, Permissions.Users);
                policy.RequireClaim(Permissions.UsersDelete, Permissions.UsersDelete);
            });
            o.AddPolicy(Permissions.UsersEdit, policy => {
                policy.RequireClaim(Piranha.Manager.Permission.Admin, Piranha.Manager.Permission.Admin);
                policy.RequireClaim(Permissions.Users, Permissions.Users);
                policy.RequireClaim(Permissions.UsersEdit, Permissions.UsersEdit);
            });
            o.AddPolicy(Permissions.UsersSave, policy => {
                policy.RequireClaim(Piranha.Manager.Permission.Admin, Piranha.Manager.Permission.Admin);
                policy.RequireClaim(Permissions.Users, Permissions.Users);
                policy.RequireClaim(Permissions.UsersSave, Permissions.UsersSave);
            });
        });        

        services.AddDbContext<Db>(dbOptions);
        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<Db>()
            .AddDefaultTokenProviders();
        services.Configure<IdentityOptions>(identityOptions != null ? identityOptions : SetDefaultOptions);
        services.ConfigureApplicationCookie(cookieOptions != null ? cookieOptions : SetDefaultCookieOptions);
        services.AddScoped<Piranha.ISecurity, IdentitySecurity>();

        return services;
    }

    /// <summary>
    /// Adds the Piranha identity module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddPiranhaIdentityWithSeed<T>(this IServiceCollection services, 
        Action<DbContextOptionsBuilder> dbOptions, 
        Action<IdentityOptions> identityOptions = null,
        Action<CookieAuthenticationOptions> cookieOptions = null) where T : class, IIdentitySeed
    {
        services = AddPiranhaIdentity(services, dbOptions, identityOptions, cookieOptions);
        services.AddScoped<IIdentitySeed, T>();

        return services;
    }

    /// <summary>
    /// Adds the Piranha identity module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddPiranhaIdentityWithSeed(this IServiceCollection services, 
        Action<DbContextOptionsBuilder> dbOptions, 
        Action<IdentityOptions> identityOptions = null,
        Action<CookieAuthenticationOptions> cookieOptions = null)
    {
        return AddPiranhaIdentityWithSeed<DefaultIdentitySeed>(services, dbOptions, identityOptions, cookieOptions);
    }

    /// <summary>
    /// Sets the default identity options if none was provided. Please note that
    /// these settings provide very LOW security in terms of password rules, but
    /// this is just so the default user can be seeded on first startup.
    /// </summary>
    /// <param name="options">The identity options</param>
    private static void SetDefaultOptions(IdentityOptions options)
    {
        // Password settings
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequiredUniqueChars = 1;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
        options.Lockout.MaxFailedAccessAttempts = 10;
        options.Lockout.AllowedForNewUsers = true;

        // User settings
        options.User.RequireUniqueEmail = true;
    }

    /// <summary>
    /// Sets the default cookie options if none was provided.
    /// </summary>
    /// <param name="options">The cookie options</param>
    private static void SetDefaultCookieOptions(CookieAuthenticationOptions options)
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.LoginPath = "/manager/login";
        options.AccessDeniedPath = "/manager/login";
        options.SlidingExpiration = true;        
    }
}