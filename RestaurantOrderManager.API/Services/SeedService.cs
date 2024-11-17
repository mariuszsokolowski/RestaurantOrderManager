using Microsoft.AspNetCore.Identity;
using RestaurantOrderManager.API.Enums;
using RestaurantOrderManager.Data;
using RestaurantOrderManager.Data.Entities;

namespace RestaurantOrderManager.API.Services
{
    internal sealed class SeedService
    {
        private readonly IConfiguration _configuration;
        private readonly DBContext _dbContext;


        public SeedService(IConfiguration configuration, DBContext context)
        {
            _configuration = configuration;
            _dbContext = context;
        }

        /// <summary>
        /// Gets the list of available commands as a formatted string.
        /// </summary>
        /// <returns>Comma-separated list of available commands.</returns>
        public List<string> GetAvailableCommands()
        {
            return Enum.GetNames(typeof(SeedDataCommandEnums)).ToList();
        }

        /// <summary>
        /// Configures the CLI commands.
        /// </summary>
        /// <param name="app">The command line application instance.</param>
        public void SeedByCLI(IServiceProvider serviceProvider, string command)
        {
            try
            {
                if (String.IsNullOrEmpty(command))
                {
                    return;
                }

                var commandList = GetAvailableCommands();
                // Try to parse the command from the enum
                if (command == SeedDataCommandEnums.ShowCommands.ToString())
                {
                    Console.WriteLine($"Available commands: {string.Join(", ", commandList)}");
                }
                else if (command == SeedDataCommandEnums.SeedData.ToString())
                {
                    // If users is not exist add user by SeedUsers
                    if (_dbContext.Set<User>().Count() < 2)
                        SeedUsers(serviceProvider);

                    using (var faker = new Fakers.Faker(_dbContext))
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            var menuFaker = faker.MenuFaker();
                            _dbContext.Set<Menu>().Add(menuFaker);
                            _dbContext.SaveChanges();

                            var notificationFaker = faker.NotificationFaker();
                            _dbContext.Set<Notification>().Add(notificationFaker);
                            _dbContext.SaveChanges();

                            var orderFaker = faker.OrderFaker();
                            var orderEntity = _dbContext.Set<Order>().Add(orderFaker);
                            _dbContext.SaveChanges();

                            var orderLineFaker = faker.OrderLineFaker(orderEntity.Entity.OrderId);
                            var orderLineEntity = _dbContext.Set<OrderLine>().Add(orderLineFaker);
                            _dbContext.SaveChanges();

                            var orderRateFaker = faker.OrderRateFaker(orderLineEntity.Entity.OrderLineId);
                            _dbContext.Set<OrderRate>().Add(orderRateFaker);
                            _dbContext.SaveChanges();

                            var waiterRateFaker = faker.WaiterRateFaker();
                            _dbContext.Set<WaiterRate>().Add(waiterRateFaker);
                            _dbContext.SaveChanges();

                        }
                        _dbContext.SaveChanges();
                        Console.WriteLine("Faker data is added.");
                    }
                }
                else if (command == SeedDataCommandEnums.SeedUsers.ToString())
                {
                    SeedUsers(serviceProvider);
                }
                else
                {
                    Console.WriteLine($"Unknown command: {command}. Available commands: {string.Join(", ", commandList)}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error on using CLI command. {e.Message}");
            }
        }


        public void SeedUsers(IServiceProvider serviceProvider)
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
                        var roleExist = RoleManager.RoleExistsAsync(roleName).Result;

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
                var _userAdmin = UserManager.FindByIdAsync(_configuration["AdminSettings:AdminUserEmail"]).Result;

                if (_userAdmin == null)
                {
                    var poweruser = new User
                    {
                        UserName = _configuration["AdminSettings:AdminUserName"],
                        Email = _configuration["AdminSettings:AdminUserEmail"],
                    };
                    string userPWD = _configuration["AdminSettings:AdminUserPassword"];
                    var createPowerUser = UserManager.CreateAsync(poweruser, userPWD).Result;
                    if (createPowerUser.Succeeded)
                    {
                        UserManager.AddToRoleAsync(poweruser, "Administrator").Wait();
                    }
                }
                var _userWaiter = UserManager.FindByEmailAsync(_configuration["AdminSettings:WaiterUserEmail"]).Result;

                if (_userAdmin == null)
                {
                    var poweruser = new User
                    {
                        UserName = _configuration["AdminSettings:WaiterUserName"],
                        Email = _configuration["AdminSettings:WaiterUserEmail"],
                    };
                    string userPWD = _configuration["AdminSettings:WaiterUserPassword"];
                    var createPowerUser = UserManager.CreateAsync(poweruser, userPWD).Result;
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
    }
}
