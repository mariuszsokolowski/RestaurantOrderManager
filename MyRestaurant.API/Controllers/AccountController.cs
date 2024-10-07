using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyRestaurant.Data.Entities;

namespace MyRestaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase

    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;


        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;

        }


        /// <summary>
        /// Logowanie do systemu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<object> Login(LoginDto model)
        {

            var result = await _signInManager.PasswordSignInAsync(model.Login, model.Password, false, false);


            if (result.Succeeded)
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.UserName == model.Login);
                if (appUser.IsBlocked)
                    return NotFound("Użytkownik zablokowany");
                //var checkRole = await _signInManager.UserManager.AddToRoleAsync(appUser,"Administrator");



                return await GenerateJwtToken(model.Login, appUser);
            }
            return NotFound("Niepoprawny login lub hasło");
            //throw new ApplicationException("INVALID_LOGIN_ATTEMPT");
        }

        /// <summary>
        /// Wylogowanie z systemu
        /// </summary>
        /// <returns></returns>
        [HttpPost("Logout")]
        public async Task<object> Logout()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            // claimsIdentity.RemoveClaim();

            //await AuthenticationManager.SignOutAsync();

            var userID = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var _user = await _userManager.FindByIdAsync(userID);
            await _signInManager.SignOutAsync();
            // await _signInManager.RefreshSignInAsync(_user);
            return Ok();

        }

        /// <summary>
        /// Rejestracja do systemu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Register")]
        public async Task<object> Register([FromBody] RegisterDto model)
        {
            var user = new User
            {
                UserName = model.Login,
                Email = model.Login
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return await GenerateJwtToken(model.Login, user);
            }

            throw new ApplicationException("UNKNOWN_ERROR");
        }

        private async Task<object> GenerateJwtToken(string email, User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            /* var userClaims = await _userManager.GetClaimsAsync(user);
             var userRoles = await _userManager.GetRolesAsync(user);
             claims.AddRange(userClaims);
             foreach (var userRole in userRoles)
             {
                 claims.Add(new Claim(ClaimTypes.Role, userRole));
                 var role = await _roleManager.FindByNameAsync(userRole);
                 if (role != null)
                 {
                     var roleClaims = await _roleManager.GetClaimsAsync(role);
                     foreach (Claim roleClaim in roleClaims)
                     {
                         claims.Add(roleClaim);
                     }
                 }
             }*/




            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );
            JWTDto jwtDto = new JWTDto()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Role = string.Join("; ", await _userManager.GetRolesAsync(user))
            };

            //return new JwtSecurityTokenHandler().WriteToken(token);
            return jwtDto;
        }

        public class LoginDto
        {

            public string Login { get; set; }


            public string Password { get; set; }

        }

        public class RegisterDto
        {
            [Required]
            public string Login { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "PASSWORD_MIN_LENGTH", MinimumLength = 6)]
            public string Password { get; set; }
        }

        private class JWTDto
        {
            [Required]
            public string Token { get; set; }

            public string Role { get; set; }
        }
    }
}