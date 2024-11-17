using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RestaurantOrderManager.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;


namespace RestaurantOrderManager.API.Filters
{
    internal sealed class AuthFilter : ResultFilterAttribute
    {
        private readonly string _name;
   


        public AuthFilter(string name)
        {
            _name = name;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
      

            var userID = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var roleName = context.HttpContext.User.Identity.Name;



            
            context.Result = new BadRequestObjectResult("You do not have permissions");
        }

    
    }
    
    
}
