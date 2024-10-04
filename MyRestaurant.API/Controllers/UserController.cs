using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyRestaurant.Data;
using MyRestaurant.Data.Repositories;
using MyRestaurant.API.Models;
using MyRestaurant.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Authorization;

namespace MyRestaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : Controller
    {
        private readonly DBContext _context;
        private UserRepository userRepo;
        private readonly UserManager<User> _userManager;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;

      
        public UserController(DBContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, UserManager<User> userManager)
        {
            _env = env;
            _context = context;

            #region AddRepository
            userRepo = new UserRepository(_context);
            #endregion
            _userManager = userManager;
        }

        /// <summary>
        /// Pobranie wszystkich użytkowników systemu
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public IActionResult Get()
        {
            try
            {
                var users = userRepo.All;
               
                // var users = _context.Users.Include(x=>x.UserRoles).Where(x => x.Roles.Select(y => y.Id).Contains(roleId));
               // Newtonsoft.Json.JsonConvert.SerializeObject(users);
                return Ok(userRepo.All);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        /// <summary>
        /// Pobranie użytkownika po identyfikatorze
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var result = userRepo.FindById(id);

            if (result != null)
                return NotFound(id);

            return Ok();
        }


        /// <summary>
        /// Dodanie użytkownika do systemu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async /*IActionResult*/ Task<object> Post([FromBody] UserModels model)

        {
            if (!ModelState.IsValid)
            {
                return   BadRequest(model);
            }

        

            try
            {
                /*userRepo.Insert(model);
                userRepo.Save();*/

                //using (IDbContextTransaction dbTransaction = _context.Database.BeginTransaction())
                //{

                    try
                    {
                        var user = new User
                        {
                            UserName = model.UserName,
                            Email = model.UserName
                        };

                        var result = await _userManager.CreateAsync(user, model.Password);
                  
                        if (result.Succeeded)
                        {

                        var _user =  await _userManager.FindByNameAsync(model.UserName);
                        _user.FirstName = model.FirstName;
                        _user.LastName = model.LastName;
                        var resultUpdate = await _userManager.UpdateAsync(_user);
                        if (resultUpdate.Succeeded)
                        {
                            var roleSatus = await _userManager.AddToRoleAsync(_user, "Client");
                            if (roleSatus.Succeeded)
                                return Ok();


                          
                        }
                        return NotFound();
                    }
                        else
                        {
                            //dbTransaction.Rollback();
                            return NotFound(result.Errors);
                        }
                       
                    }
                    catch (Exception e)
                    {
                        //dbTransaction.Rollback();
                        return NotFound(e.Message);
                    }

                //}

           

           
          
         
            }

            catch (Exception e)
            {
                return NotFound(e.Message);
            }

        }

        /// <summary>
        /// Aktualizacja użytkownika
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        [HttpPut("")]
        public async Task<object> Put(UserModels model)
        {

            if (!ModelState.IsValid)
                return BadRequest(model);
            //if (model.UserName == "admin")
            //    return NotFound();
        
            try
            {
                var _user = await _userManager.FindByIdAsync(model.UserId);
                if (model.Password.Length > 0)
                {
                    foreach (var item in _userManager.PasswordValidators)
                    {
                        var result = item.ValidateAsync(_userManager,_user, model.Password).Result.Succeeded;
                        if(!result)
                        { return NotFound(); }

                    }
                }
                _user.UserName = model.UserName;
                _user.FirstName = model.FirstName;
                _user.LastName = model.LastName;
                if (model.RoleName.Length > 0)
                {
                    var allUserRoles = await _userManager.GetRolesAsync(_user);
                    var resultRemove = await _userManager.RemoveFromRolesAsync(_user, allUserRoles);
                    var result = await _userManager.AddToRoleAsync(_user, model.RoleName);
                   
                }
                if(model.Password.Length>0)
                {
                    var result =  _userManager.PasswordHasher.HashPassword(_user,model.Password);
                    _user.PasswordHash = result;
                    
                }
               var _resultUpdate = await _userManager.UpdateAsync(_user);
                ///userRepo.Update(_user);
               //  userRepo.Save();
               if(_resultUpdate.Succeeded)
                return Ok(model);
                return NotFound();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        
        }


        /// <summary>
        /// Usunięcie użyutkownika z systemu
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("")]
        public async Task<object> Delete(string id)
        {
            return BadRequest();
            if (id.Length<= 0)
                return BadRequest();
            try
            {
                var _user = await _userManager.FindByIdAsync(id);
                var result = await _userManager.DeleteAsync(_user);
                if(result.Succeeded)
                return Ok();
                return NotFound();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

        }

        /// <summary>
        /// Zablokowanie użytkownika
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("Deactivate")]
        public async Task<object> Deactivate(UserModels model)
        {
            
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                var user = userRepo.All.Where(x => x.UserName == model.UserName).FirstOrDefault();
                if (user==null)
                return NotFound();
                user.IsBlocked = true;
                userRepo.Update(user);
                userRepo.Save();
                return Ok("Użytkownik "+model.UserName +" został zablokowany");

            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

        }
        [HttpPut("Activate")]
        public async Task<object> Activate(UserModels model)
        {

            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                var user = userRepo.All.Where(x => x.UserName == model.UserName).FirstOrDefault();
                if (user == null)
                    return NotFound();
                user.IsBlocked = false;
                userRepo.Update(user);
                userRepo.Save();
                return Ok("Użytkownik " + model.UserName + " został odblokowany");

            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

        }

        /// <summary>
        /// Pobranie zdjęcia użytkownika
        /// </summary>
        /// <param name="imgName"></param>
        /// <returns></returns>
        [HttpGet("GetAvatar/{imgName}")]
        public IActionResult GetAvatar(string imgName)
        {
            if (imgName == "null" || imgName.Length == 0)
            {
                imgName = "DefaultAvatar.png";
            }
                try
            {
                var path = _env.WebRootPath + "\\Avatars\\" + imgName;
                var image = System.IO.File.OpenRead(path);
                return File(image, "image/jpeg");
            }
            catch 
            { return null; }
        }

        /// <summary>
        /// Aktualizacja zdjęcia użytkownika
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("UpdateFile")]
        public IActionResult UpdateFile(IFormFile file)
        {




            try
            {
                ContentDisposition contentDisposition = new ContentDisposition(file.ContentDisposition);

                var path = _env.WebRootPath + "\\Avatars\\" + contentDisposition.FileName;

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                    // file.CopyToAsync(stream);
                }
                var idArray = contentDisposition.FileName.Split('.');
                int id = Int32.Parse(idArray[0]);
                var model = userRepo.FindById(id);
                //model.Avatar = contentDisposition.FileName;
                userRepo.Update(model);
                userRepo.Save();


                return Ok();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

        }


        /// <summary>
        /// Pobranie wszystkich uprawnień systemu
        /// </summary>
        /// <returns></returns>
        [HttpGet("Roles")]
        public IActionResult GetAllRoles()
        {
 
            return Ok(_context.Roles.Select(x=>x.Name).ToList());
        }

        /// <summary>
        /// Pobranie wszystkich użytkowników zdefiniowanych jako Klient
        /// </summary>
        /// <returns></returns>
        [HttpGet("Clients")]
        public IActionResult GetAllClients()
        {
            try
            {
                
                return Ok(
                    userRepo.All.Where(x => x.UserRoles.First().Role.Name == "Client").OrderBy(x=>x.UserName).Select(x => x.UserName).ToList()
                    );
            }
            catch
            {
                return NoContent();
            }
        
        }




    }
}