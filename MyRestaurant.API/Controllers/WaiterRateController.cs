﻿using System;
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
    [Authorize]
    public class WaiterRateController : ControllerBase
    {
        private WaiterRateRepository reporWaiterRate;
        private readonly DBContext _context;

        public WaiterRateController(DBContext context)
        {
            _context = context;
            reporWaiterRate = new WaiterRateRepository(context);
        }


        /// <summary>
        /// Get waiter rate list
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public IActionResult Get()
        {
            IQueryable<WaiterRate> result = null;
            string user = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (user != null)
            {
                 result = reporWaiterRate.All.Where(x => x.UserSign == user  && x.Cancled == false && x.Rating <= 0);
           
            }
            return Ok(result);

        }

        /// <summary>
        /// Cancel all waiter rates
        /// </summary>
        /// <returns></returns>
        [HttpPost("CancelAll")]
        public IActionResult CancelAll()
        {

            string user = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var entity = reporWaiterRate.All.Where(x => x.UserSign == user && x.Cancled == false && x.Rating <= 0);
            foreach (var item in entity)
            {
                item.Cancled = true;
                reporWaiterRate.Update(item);
            }
            reporWaiterRate.Save();
            return Ok();


        }

        /// <summary>
        /// Cancel waiter rate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Cancel")]
        public IActionResult Cancel([FromBody] WaiterRateModel model)
        {

            string user = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var entity = reporWaiterRate.All.Where(x => x.UserSign == user && x.Cancled == false && x.Rating <= 0 && x.WaiterRateID == model.WaiterRateID);
            if (entity.Count() <= 0)
            {
                return NotFound();
            }
            foreach (var item in entity)
            {
                item.Cancled = true;
                reporWaiterRate.Update(item);
            }
            reporWaiterRate.Save();
            return Ok("Rate has been canceled");

        }

        /// <summary>
        /// Add waiter rate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Rate")]
        public IActionResult Rate([FromBody] WaiterRateModel model)
        {

            string user = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var entity = reporWaiterRate.All.Where(x => x.UserSign == user && x.Cancled == false && x.Rating <= 0 && x.WaiterRateID == model.WaiterRateID);
            if (entity.Count() <= 0)
            {
                return NotFound();
            }
            foreach (var item in entity)
            {
                item.Rating = model.Rating;
                reporWaiterRate.Update(item);
            }
            reporWaiterRate.Save();
            return Ok("Thank you for rating our waiter");

        }


        public class WaiterRateModel
        {
            public int WaiterRateID { get; set; }
            public decimal Rating { get; set; }
        }
    }
}