using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;


namespace MyRestaurant.Client
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
            /*   services.Configure<CookiePolicyOptions>(options =>
               {
                   // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                   options.CheckConsentNeeded = context => true;
                   options.MinimumSameSitePolicy = SameSiteMode.None;
               });

               services.AddAuthentication(options => {
                   options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                   options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                   options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                   options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                   options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
               })
               .AddOpenIdConnect(options => {
                   options.Authority = "https://localhost:44389";
                   options.RequireHttpsMetadata = false;
                   options.ClientId = "mvc";
                   options.SignedOutRedirectUri = "/";
                   options.SaveTokens = true;
                   options.Events = new OpenIdConnectEvents
                   {
                       OnRemoteFailure = context =>
                       {
                           context.Response.Redirect("/");
                           context.HandleResponse();
                           return Task.CompletedTask;
                       }
                   };
               })
               .AddCookie()
               .AddJwtBearer(options => {
                   options.Authority = "https://localhost:44389";
                   options.RequireHttpsMetadata = false;
                   options.Audience = "api1";
               });*/

    

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                   app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                    routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
