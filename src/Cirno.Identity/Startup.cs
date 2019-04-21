// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using AutoMapper;
using Cirno.Identity.Data;
using Cirno.Identity.Diagnostics;
using Cirno.Identity.Models;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Cirno.Identity
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContextPool<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("IdentitiesDb"), npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(migrationsAssembly);
                });
                if (Environment.IsDevelopment())
                    options.EnableSensitiveDataLogging();
            }, 32);

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAutoMapper();

            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);

            var idsrvBuilder = services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                // this adds the config data from DB (clients, resources)
                .AddConfigurationStore(options =>
                {
                    var connectionString = Configuration.GetConnectionString("ConfigurationDb");
                    options.ConfigureDbContext = builder =>
                    {
                        builder.UseNpgsql(connectionString, npgsqlOptions =>
                        {
                            npgsqlOptions.MigrationsAssembly(migrationsAssembly);
                        });
                        if (Environment.IsDevelopment())
                            builder.EnableSensitiveDataLogging();
                    };
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    var connectionString = Configuration.GetConnectionString("OperationalDb");
                    options.ConfigureDbContext = builder =>
                    {
                        builder.UseNpgsql(connectionString, npgsqlOptions =>
                        {
                            npgsqlOptions.MigrationsAssembly(migrationsAssembly);
                        });
                        if (Environment.IsDevelopment())
                            builder.EnableSensitiveDataLogging();
                    };
                })
                .AddAspNetIdentity<ApplicationUser>();

            if (Environment.IsDevelopment())
            {
                idsrvBuilder.AddDeveloperSigningCredential();

                idsrvBuilder.AddInMemoryIdentityResources(Config.GetIdentityResources());
                idsrvBuilder.AddInMemoryApiResources(Config.GetApis());
                idsrvBuilder.AddInMemoryClients(Config.GetClients());
            }
            else
            {
                var certificateLocation = Configuration["IS4:Signing:Certificate"];
                var certificatePassword = Configuration["IS4:Signing:Password"];
                X509Certificate2 certificate = new X509Certificate2(certificateLocation, certificatePassword);
                idsrvBuilder.AddSigningCredential(certificate);
            }
            services.AddCors();
            services.AddAuthentication();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowCredentials();
            });

            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseHttpsRedirection();
            app.UseMvcWithDefaultRoute();
        }
    }
}