using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Swashbuckle.AspNetCore.Swagger;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using MyRestaurant.Data.Entities;
using MyRestaurant.API.Enums;

namespace MyRestaurant.API
{
    public class Startup
    {
        #region Fields
        private string _command;
        public IConfiguration Configuration { get; }
        #endregion

        #region Constructors
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _command = Configuration.GetValue<string>("seed");
        }
        #endregion

        #region Methods

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
            #region Servvices
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

            services.AddCors(options => options.AddPolicy("MyPolicy",
         builder =>
         {
             builder.AllowAnyMethod().AllowAnyHeader()
                    .WithOrigins($"{Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>()}")
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod(); ;
         }));

            services.AddDbContext<Data.DBContext>(options => options.UseMySql(Configuration.GetConnectionString("MysqlConnection")));

            services.AddIdentity<User, Role>()
                             .AddDefaultUI()
                             .AddRoles<Role>()
                             .AddRoleManager<RoleManager<Role>>()
                             .AddUserManager<UserManager<User>>()
                             .AddDefaultTokenProviders()
                             .AddEntityFrameworkStores<Data.DBContext>();

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
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider _serviceProvider)
        {
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

            if (!String.IsNullOrEmpty(_command))
                SeedByCLI(_serviceProvider);

        }


        #region Seeds

        /// <summary>
        /// Gets the list of available commands as a formatted string.
        /// </summary>
        /// <returns>Comma-separated list of available commands.</returns>
        private List<string> GetAvailableCommands()
        {
            return Enum.GetNames(typeof(SeedDataCommandEnums)).ToList();
        }

        /// <summary>
        /// Configures the CLI commands.
        /// </summary>
        /// <param name="app">The command line application instance.</param>
        public void SeedByCLI(IServiceProvider serviceProvider)
        {
            try
            {
                if (String.IsNullOrEmpty(_command))
                {
                    return;
                }

                var commandList = GetAvailableCommands();
                // Try to parse the command from the enum
                if (_command == SeedDataCommandEnums.ShowCommands.ToString())
                {
                    Console.WriteLine($"Available commands: {string.Join(", ", commandList)}");
                }
                else if (_command == SeedDataCommandEnums.SeedData.ToString())
                {

                }
                else if (_command == SeedDataCommandEnums.SeedUsers.ToString())
                {
                    SeedUsers(serviceProvider);
                }
                else
                {
                    Console.WriteLine($"Unknown command: {_command}. Available commands: {string.Join(", ", commandList)}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error on using CLI command. {e.Message}");
            }
        }

        private async void SeedUsers(IServiceProvider serviceProvider)
        {
            //initializing custom roles 
            try
            {
                var RoleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
                var UserManager = serviceProvider.GetRequiredService<UserManager<User>>();
                string[] roleNames = { "Administrator", "Waiter", "Cook", "Client" };
                foreach (var roleName in roleNames)
                {
                    try
                    {
                        var roleExist = await RoleManager.RoleExistsAsync(roleName);

                        if (roleExist == false)
                        {
                            //create the roles and seed them to the database: Question 1
                            var roleResult = RoleManager.CreateAsync(new Role() { Name = roleName });
                            roleResult.Wait();
                        }
                        else
                        {
                            Console.WriteLine($"The role already exists {roleName}");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error when created Role {e.Message}");
                    }
                }
                //Here you could create a super user who will maintain the web app
                var poweruser = new User
                {
                    UserName = Configuration["AdminSettings:UserName"],
                    Email = Configuration["AdminSettings:UserEmail"],
                };
                //Ensure you have these values in your appsettings.json file
                string userPWD = Configuration["AdminSettings:UserPassword"];
                var _user = await UserManager.FindByEmailAsync(Configuration["AdminSettings:UserEmail"]);

                if (_user == null)
                {
                    var createPowerUser = await UserManager.CreateAsync(poweruser, userPWD);
                    if (createPowerUser.Succeeded)
                    {
                        await UserManager.AddToRoleAsync(poweruser, "Administrator");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #endregion
    }
}
