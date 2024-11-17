using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RestaurantOrderManager.Data.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;
using RestaurantOrderManager.Data;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RestaurantOrderManager.API.Automapper;
using RestaurantOrderManager.API.Services;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
#region AddAutomMapper
var config = new AutoMapper.MapperConfiguration(cfg =>
{
    cfg.AddProfile(new AutomapperProfile());

});
var mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);
#endregion



#region Services
builder.Services.AddScoped<SeedService>();
builder.Services.AddControllers()
.AddNewtonsoftJson(options =>
 {
     options.SerializerSettings.ContractResolver
                     = new Newtonsoft.Json.Serialization.DefaultContractResolver();
     options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
 });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "API Restauracji",
        Description = "API MyRestaurant",
        TermsOfService = new Uri("http://localhost")
    });
    //var xmlPath = System.AppDomain.CurrentDomain.BaseDirectory + @"MyRestaurant.API.xml";
    //c.IncludeXmlComments(xmlPath);

});
var allowOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();
Console.WriteLine($"Allow Orgin {string.Join(", ", allowOrigins)}");
builder.Services.AddCors(options => options.AddPolicy("MyPolicy",
builder =>
{
    builder.AllowAnyMethod().AllowAnyHeader()
    .WithOrigins(string.Join(", ", allowOrigins))
    .AllowCredentials()
    .AllowAnyHeader()
        .AllowAnyMethod(); ;
}));

string connectionString = builder.Configuration.GetConnectionString("MysqlConnection");
builder.Services.AddDbContext<DBContext>(options =>
options.UseMySql( connectionString,
ServerVersion.AutoDetect(connectionString), b => b.MigrationsAssembly("RestaurantOrderManager.API"))
);



builder.Services.AddIdentity<User, Role>()
.AddDefaultUI()
                 .AddRoles<Role>()
                 .AddRoleManager<RoleManager<Role>>()
                 .AddUserManager<UserManager<User>>()
                 .AddDefaultTokenProviders()
                 .AddEntityFrameworkStores<DBContext>();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
builder.Services
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
            ValidIssuer = builder.Configuration["JwtIssuer"],
            ValidAudience = builder.Configuration["JwtIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtKey"])),
            ClockSkew = TimeSpan.Zero // remove delay of token when expire

        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Administrator"));
});
builder.Services.AddSignalR();
#endregion
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

var serviceProvider = app.Services.CreateScope().ServiceProvider;

var seedService = serviceProvider.GetRequiredService<SeedService>();
var command = builder.Configuration["seed"];

seedService.SeedByCLI(serviceProvider, command);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


