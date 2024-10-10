using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyRestaurant.Data;
using MyRestaurant.Data.Entities;
using MyRestaurant.Data.Repositories;

namespace MyRestaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   [Authorize(AuthenticationSchemes = "Bearer")]
    public class RateController : ControllerBase
    {
        #region Repository
        private OrderRateRepository repoRate;
        private OrderRepository repoOrder;
        #endregion

        private readonly DBContext _context;

        public RateController(DBContext context)
        {
            _context = context;
            repoOrder = new OrderRepository(context);
            repoRate = new OrderRateRepository(context);
        }

        /// <summary>
        /// Get menu rate list
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public IActionResult GetMenu()
        {
      

            IQueryable<OrderRate> result = null;
            string user = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (user!=null)
            {
                result = repoRate.All.Where(x => x.UserSign == user && x.Type == Data.Enums.RatingType.Menu && x.Cancled==false && x.Rating<=0);
            }
            return Ok(result);

        }

        /// <summary>
        /// Cancel all menu rate
        /// </summary>
        /// <returns></returns>
        [HttpPost("CancelAll")]
        public IActionResult CancelAllMenu()
        {
      
            string user = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var entity = repoRate.All.Where(x => x.UserSign == user && x.Cancled == false && x.Rating <= 0);
            foreach(var item in entity)
            {
                item.Cancled = true;
                repoRate.Update(item);
            }
            repoRate.Save();
            return Ok();


        }

        /// <summary>
        /// Cancel menu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Cancel")]
        public IActionResult CancelMenu([FromBody] RateMenuModel model)
        {

            string user = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var entity = repoRate.All.Where(x => x.UserSign == user && x.Cancled == false && x.Rating <= 0 && x.RateID==model.RateID);
            if(entity.Count()<=0)
            {
                return NotFound();
            }
            foreach (var item in entity)
            {
                item.Cancled = true;
                repoRate.Update(item);
            }
            repoRate.Save();
            return Ok();

        }

        /// <summary>
        /// Rate menu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("RateMenu")]
        public IActionResult RateMenu([FromBody] RateMenuModel model)
        {

            string user = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var entity = repoRate.All.Where(x => x.UserSign == user && x.Cancled == false && x.Rating <= 0 && x.RateID == model.RateID);
            if (entity.Count() <= 0)
            {
                return NotFound();
            }
            foreach (var item in entity)
            {

                item.Rating = model.Rating;
                repoRate.Update(item);
            }
            repoRate.Save();
            return Ok();

        }


        public class RateMenuModel
        {
            public int RateID { get; set; }
            public decimal Rating { get; set; }
        }
    }
}