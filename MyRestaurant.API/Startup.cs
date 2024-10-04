using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Owin;
using Owin;
using System.Web;
using System.Web.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Swashbuckle.AspNetCore.Swagger;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using MyRestaurant.Data.Entities;
using MyRestaurant.API.Filters;
using System.Reflection;
using System.IO;

namespace MyRestaurant.API
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


            #region AddAutomMapper
            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new Automapper.AutomapperProfile());

            });
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            #endregion

            services.AddMvc()
             .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver
                        = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "API Restauracji",
                    Description = "API MyRestaurant",
                    TermsOfService = "-"
                });
                //Ustawianie opisów do API
                var xmlPath = System.AppDomain.CurrentDomain.BaseDirectory + @"MyRestaurant.API.xml";
                c.IncludeXmlComments(xmlPath);

            });






            /*services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));*/

            services.AddCors(options => options.AddPolicy("MyPolicy",
         builder =>
         {
             builder.AllowAnyMethod().AllowAnyHeader()
                    .WithOrigins("https://localhost:5001")
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod(); ;
         }));



            // services.AddDbContext<Data.DBContext>(options => options.UseMySql(Configuration.GetConnectionString("MysqlConnection"), b => b.MigrationsAssembly("MyRestaruant.Data")));
            services.AddDbContext<Data.DBContext>(options => options.UseMySql(Configuration.GetConnectionString("MysqlConnection")));


            /* services.AddIdentity<User, IdentityRole>()
                     .AddRoleManager<RoleManager<IdentityRole>>()
                     .AddDefaultUI()
                     .AddDefaultTokenProviders()
                     .AddEntityFrameworkStores<Data.DBContext>();*/

            //services.AddDefaultIdentity<IdentityUser>().AddRoles<IdentityRole>();


            services.AddIdentity<User, Role>()
                             .AddDefaultUI()
                             .AddRoles<Role>()
                             .AddRoleManager<RoleManager<Role>>()
                             .AddDefaultTokenProviders()
                             .AddEntityFrameworkStores<Data.DBContext>();


            //services.AddScoped<IRoleValidator<IdentityRole>, RoleValidator<IdentityRole>>();
            // services.AddScoped<RoleManager<IdentityRole>, RoleManager<IdentityRole>>();


            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;


                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = Configuration["JwtIssuer"],
                        ValidAudience = Configuration["JwtIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire

                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Administrator"));
            });
            services.AddSignalR();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            /* var builder = services.AddIdentityServer(options =>
             {
                 options.IssuerUri = "MyIdentityServer";
                 options.Events.RaiseErrorEvents = true;
                 options.Events.RaiseInformationEvents = true;
                 options.Events.RaiseFailureEvents = true;
                 options.Events.RaiseSuccessEvents = true;
             })
                .AddTestUsers(new List<TestUser> {
                     new TestUser{SubjectId = "1", Username = "zerokoll", Password = "test",
                         Claims =
                         {
                             new Claim(JwtClaimTypes.Name, "Chris Klug"),
                             new Claim(JwtClaimTypes.GivenName, "Chris"),
                             new Claim(JwtClaimTypes.FamilyName, "Klug"),
                             new Claim(JwtClaimTypes.Email, "chris@59north.com"),
                         }
                     }
                });

             // in-memory, json config
             builder.AddInMemoryIdentityResources(Configuration.GetSection("IdentityResources"));
             builder.AddInMemoryApiResources(Configuration.GetSection("ApiResources"));
             builder.AddInMemoryClients(Configuration.GetSection("clients"));

             if (Environment.IsDevelopment())
             {
                 builder.AddDeveloperSigningCredential();
             }
             else
             {
                 throw new Exception("need to configure key material");
             }

             AccountOptions.AllowRememberLogin = false;
             AccountOptions.AutomaticRedirectAfterSignOut = true;
             AccountOptions.ShowLogoutPrompt = false;
             AccountOptions.WindowsAuthenticationEnabled = false;

     */

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider _serviceProvider)
        {
            //CreateRoles(_serviceProvider);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseCors("MyPolicy");
            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                routes.MapHub<SignalR.RateHub>("/rateHub");
                routes.MapHub<SignalR.OrderHub>("/orderHub");
                routes.MapHub<SignalR.NotificationHub>("/notificationHub");

            });

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            CreateRoles(_serviceProvider);





        }

        /*public void ConfigureOAuth(IAppBuilder app)
        {


            //Identity
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseWebApi(config);
        }*/


        /// <summary>
        /// Używamy tylko podczas tworzenia nowych ról
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            //initializing custom roles 
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();



            string[] roleNames = { "Administrator", "Waiter", "Cook", "Client" };


            foreach (var roleName in roleNames)
            {
                try
                {

                    var roleExist = RoleManager.RoleExistsAsync(roleName);

                    if (roleExist == null)
                    {
                        //create the roles and seed them to the database: Question 1
                        var roleResult = RoleManager.CreateAsync(new IdentityRole(roleName));
                        roleResult.Wait();
                    }
                }
                catch (Exception e)
                {
                    var bug = e.Message;
                }
            }

            //Here you could create a super user who will maintain the web app
            var poweruser = new IdentityUser
            {

                UserName = Configuration["AppSettings:UserName"],
                Email = Configuration["AppSettings:UserEmail"],
            };
            //Ensure you have these values in your appsettings.json file
            string userPWD = Configuration["AppSettings:UserPassword"];
            var _user = await UserManager.FindByEmailAsync(Configuration["AppSettings:AdminUserEmail"]);

            if (_user == null)
            {
                var createPowerUser = await UserManager.CreateAsync(poweruser, userPWD);
                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the role
                    await UserManager.AddToRoleAsync(poweruser, "Admin");

                }
            }
        }


    }
}
