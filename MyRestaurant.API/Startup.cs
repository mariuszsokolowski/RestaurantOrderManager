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
using Bogus;
using System.Reflection;
using MyRestaurant.Data;
using System.ComponentModel.DataAnnotations.Schema;

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
            var allowOrigins = Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();
            Console.WriteLine($"Allow Orgin {string.Join(", ", allowOrigins)}");
            services.AddCors(options => options.AddPolicy("MyPolicy",
         builder =>
         {
             builder.AllowAnyMethod().AllowAnyHeader()
                    .WithOrigins(string.Join(", ", allowOrigins))
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider _serviceProvider, DBContext context)
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
                SeedByCLI(_serviceProvider, context);

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
        public void SeedByCLI(IServiceProvider serviceProvider, DBContext context)
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
                    // If users is not exist add user by SeedUsers
                    if (context.Set<User>().Count()<2)
                        SeedUsers(serviceProvider);

                    using (var faker = new Fakers.Faker(context))
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            var menuFaker = faker.MenuFaker();
                            context.Set<Menu>().Add(menuFaker);
                            context.SaveChanges();

                            var notificationFaker = faker.NotificationFaker();
                            context.Set<Notification>().Add(notificationFaker);
                            context.SaveChanges();

                            var orderFaker = faker.OrderFaker();
                            var orderEntity = context.Set<Order>().Add(orderFaker);
                            context.SaveChanges();

                            var orderLineFaker = faker.OrderLineFaker(orderEntity.Entity.OrderId);
                            var orderLineEntity = context.Set<OrderLine>().Add(orderLineFaker);
                            context.SaveChanges();

                            var orderRateFaker = faker.OrderRateFaker(orderLineEntity.Entity.OrderLineId);
                            context.Set<OrderRate>().Add(orderRateFaker);
                            context.SaveChanges();

                            var waiterRateFaker = faker.WaiterRateFaker();
                            context.Set<WaiterRate>().Add(waiterRateFaker);
                            context.SaveChanges();

                        }
                        context.SaveChanges();
                        Console.WriteLine("Faker data is added.");
                    }
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

    
        private  void SeedUsers(IServiceProvider serviceProvider)
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
                        var roleExist =  RoleManager.RoleExistsAsync(roleName).Result;

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
                #region Admin

              

                var _userAdmin =  UserManager.FindByIdAsync(Configuration["AdminSettings:AdminUserEmail"]).Result;

                if (_userAdmin == null)
                {
                    var poweruser = new User
                    {
                        UserName = Configuration["AdminSettings:AdminUserName"],
                        Email = Configuration["AdminSettings:AdminUserEmail"],
                    };
                    string userPWD = Configuration["AdminSettings:AdminUserPassword"];
                    var createPowerUser =  UserManager.CreateAsync(poweruser, userPWD).Result;
                    if (createPowerUser.Succeeded)
                    {
                         UserManager.AddToRoleAsync(poweruser, "Administrator").Wait();
                    }
                }
                var _userWaiter =  UserManager.FindByEmailAsync(Configuration["AdminSettings:WaiterUserEmail"]).Result;

                if (_userAdmin == null)
                {
                    var poweruser = new User
                    {
                        UserName = Configuration["AdminSettings:WaiterUserName"],
                        Email = Configuration["AdminSettings:WaiterUserEmail"],
                    };
                    string userPWD = Configuration["AdminSettings:WaiterUserPassword"];
                    var createPowerUser =  UserManager.CreateAsync(poweruser, userPWD).Result;
                    if (createPowerUser.Succeeded)
                    {
                         UserManager.AddToRoleAsync(poweruser, "Waiter").Wait();
                    }
                }
                #endregion


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
