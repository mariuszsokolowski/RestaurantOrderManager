using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyRestaurant.Data;
using MyRestaurant.Data.Repositories;
using MyRestaurant.Data.Entities;
using System.IO;
using System.Net.Mime;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MyRestaurant.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {

        private readonly DBContext _context;
        private MenuRepository repoMenu;
        private OrderLineRepository repoOrderLine;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;



        public MenuController(DBContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            _env = env;
            _context = context;

            #region AddRepository
            repoMenu = new MenuRepository(_context);
            repoOrderLine = new OrderLineRepository(_context);
            #endregion

        }

        /// <summary>
        /// Get menu list
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public IActionResult Get()
        {
            var result = repoMenu.AllWithRating;
            if (result.Count() <= 0)
                return NoContent();
            return Ok(result);
        }

        /// <summary>
        /// Get menu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var result = repoMenu.FindById(id);
            if (result != null)
                return NotFound(id);
            return Ok();
        }

        /// <summary>
        /// Add menu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("")]
        public IActionResult Post([FromBody] Menu model)

        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }



            try
            {
                repoMenu.Insert(model);
                repoMenu.Save();
                return Ok(model);
            }

            catch (Exception e)
            {
                return NotFound(e.Message);
            }

        }

        /// <summary>
        /// Update menu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        [HttpPut("")]
        public IActionResult Put(Menu model)
        {

            if (!ModelState.IsValid || model.MenuId <= 0)
                return BadRequest(model);


            try
            {



                repoMenu.Update(model);
                repoMenu.Save();

                return Ok("Zmiany zostały zapisane.");
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

        }

        /// <summary>
        /// Unlock menu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Activate")]
        public IActionResult Activate(Menu model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }
            try
            {
                var entity = repoMenu.FindById(model.MenuId);
                entity.Inactive = false;
                repoMenu.Update(entity);
                repoMenu.Save();
                return Ok("Danie " + model.Name + " zostało odblokowane.");
            }
            catch
            {
                return BadRequest(model);
            }


        }

        /// <summary>
        /// Deactivation Menu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Deactivate")]
        public IActionResult Deactivate(Menu model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }
            try
            {
                var entity = repoMenu.FindById(model.MenuId);
                entity.Inactive = true;
                repoMenu.Update(entity);
                repoMenu.Save();
                return Ok("Danie " + model.Name + " zostało zablokowane.");
            }
            catch
            {
                return BadRequest(model);
            }


        }

        /// <summary>
        /// Update menu image
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("UpdateFile")]
        public IActionResult UpdateFile(IFormFile file)
        {


            try
            {
                ContentDisposition contentDisposition = new ContentDisposition(file.ContentDisposition);

                var path = _env.WebRootPath + "\\Images\\" + contentDisposition.FileName;

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                    // file.CopyToAsync(stream);
                }
                var idArray = contentDisposition.FileName.Split('.');
                int id = Int32.Parse(idArray[0]);
                var model = repoMenu.FindById(id);
                model.Image = contentDisposition.FileName;
                repoMenu.Update(model);
                repoMenu.Save();


                return Ok();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

        }


        /// <summary>
        /// Delete menu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
                return BadRequest();
            try
            {
                if (repoOrderLine.All.Where(x => x.MenuId == id).Count() > 0)
                {
                    return NotFound("This menu cannot be deleted. There are bindings.");
                }

                repoMenu.Delete(id);
                repoMenu.Save();

                return Ok();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

        }

        /// <summary>
        /// Get menu image
        /// </summary>
        /// <param name="imgName"></param>
        /// <returns></returns>
        [HttpGet("GetImg/{imgName}")]
        public IActionResult GetImg(string imgName)
        {
            try
            {
                var path = _env.WebRootPath + "\\Images\\" + imgName;
                var image = System.IO.File.OpenRead(path);
                return File(image, "image/jpeg");
            }
            catch
            {
                return null;
            }
        }



    }
}