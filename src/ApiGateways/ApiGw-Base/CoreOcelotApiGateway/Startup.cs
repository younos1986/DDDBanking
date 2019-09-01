using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Ocelot.Authorization;
using Core.Ocelot.IPRateLimiters;
using Core.Ocelot.OcelotMiddlewares;
using CoreOcelotApiGateway.Infrastructure.AuthorizationHandler;
using CoreOcelotApiGateway.Infrastructure.Authorizer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CoreOcelotApiGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            AddAuthorization(services);


            services.AddScoped<CustomCoreOcelotAuthorizer, CustomCoreOcelotAuthorizer>();
            services.AddCoreOcelot(config =>
            {
                config.EnableAutorization = false;
                config.CoreOcelotAuthorizer = new CustomCoreOcelotAuthorizer(
                    Configuration,
                    services.BuildServiceProvider().GetService<IMemoryCache>()
                    );

                // in case to disable IPRateLimiting - no configuraion part as "IPRateLimitingSetting" is needed 
                config.IPRateLimitingSetting = new IPRateLimitingSetting() { EnableEndpointRateLimiting = false };
                //config.IPRateLimitingSetting = Configuration.GetSection("IPRateLimitingSetting").Get<IPRateLimitingSetting>();

            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();

            //ignore Ocelat ReRoutes
            AppMapWhen(app);

            app.UseCoreOcelot();


            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void AppMapWhen(IApplicationBuilder app)
        {
            List<string> ignoreList = new List<string>()
            {
                "/robots.txt",
                "/favicon.ico",
                "/api/CustomValues",
                "/api/UserAccess"
            };

            //Add the specific route to app.MapWhen To ignore Ocelat route capturing.
            app.MapWhen((context) =>
            {
                return ignoreList.Any(q => context.Request.Path.StartsWithSegments(q));
            }, (appBuilder) =>
            {
                appBuilder.UseMvc();
            });
        }


        private void AddAuthorization(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })

                           .AddJwtBearer(cfg =>
                           {
                               //  cfg.RequireHttpsMetadata = false;
                               cfg.SaveToken = true;
                               cfg.TokenValidationParameters = new TokenValidationParameters
                               {
                                   ValidIssuer = Configuration["BearerTokens:Issuer"],
                                   ValidAudience = Configuration["BearerTokens:Audience"],
                                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["BearerTokens:Key"])),
                                   // ValidateIssuerSigningKey = false,
                                   //  ValidateLifetime = true,
                                   // ClockSkew = TimeSpan.Zero

                                   ValidateIssuer = true,
                                   ValidateAudience = true,
                                   ValidateLifetime = true,
                                   ValidateIssuerSigningKey = true,
                                   //   ValidIssuer = Configuration["Jwt:Issuer"],
                                   //  ValidAudience = Configuration["Jwt:Issuer"],
                                   //   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))


                               };
                               cfg.Events = new JwtBearerEvents
                               {
                                   OnAuthenticationFailed = context =>
                                   {
                                       var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                                       logger.LogError("Authentication failed.", context.Exception);
                                       return Task.CompletedTask;
                                   },
                                   OnTokenValidated = context =>
                                   {

                                       // return true;
                                       var tokenValidatorService = context.HttpContext.RequestServices.GetRequiredService<ITokenValidatorService>();
                                       return tokenValidatorService.ValidateAsync(context);
                                   },
                                   OnMessageReceived = context =>
                                   {
                                       return Task.CompletedTask;
                                   },
                                   OnChallenge = context =>
                                   {
                                       var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                                       logger.LogError("OnChallenge error", context.Error, context.ErrorDescription);
                                       return Task.CompletedTask;
                                   }
                               };
                           });

            AddDynamicPermissionsPolicy(services);
        }

        private static void AddDynamicPermissionsPolicy(IServiceCollection services)
        {
            services.AddScoped<IAuthorizationHandler, DynamicPermissionsAuthorizationHandler>();
            services.AddAuthorization(opts =>
            {
                opts.AddPolicy(
                name: ConstantPolicies.DynamicPermission,
                configurePolicy: policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new DynamicPermissionRequirement());
                });
            });

        }

    }
}
