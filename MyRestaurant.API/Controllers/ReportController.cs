using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyRestaurant.Data;
using MyRestaurant.Data.Repositories;

namespace MyRestaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly DBContext _context;
        private OrderRepository repoOrder;
        private OrderRateRepository repoRate;
        private OrderLineRepository repoOrderLine;
        private WaiterRateRepository repoWaiterRate;
        private NotificationRepository repoNotification;
        private UserRepository repoUser;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;
        private MenuRepository repoMenu;



        public ReportController(DBContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {

            _env = env;
            _context = context;


            #region AddRepository
            repoUser = new UserRepository(_context);
            repoOrder = new OrderRepository(_context);
            repoOrderLine = new OrderLineRepository(_context);
            repoMenu = new MenuRepository(_context);
            repoRate = new OrderRateRepository(_context);
            repoWaiterRate = new WaiterRateRepository(_context);
            repoNotification = new NotificationRepository(_context);
            #endregion


        }

        private class SaleDTO
        {
            public string MenuName { get; set; }
            public int OrderCount { get; set; }
            public int OrderQuantity { get; set; }
            public double Cash { get; set; }
            public decimal Rating { get; set; }
            public double TotalProcent { get; set; }

        }
        private class EmployeeDTO
        {
            public string FullName { get; set; }
            public decimal Rating { get; set; }
            public int WaiterAssignCalls { get; set; }
            public int TotalCalls { get; set; }

        }


        /// <summary>
        /// Pobranie danych do raportu sprzedaży
        /// </summary>
        /// <returns></returns>
        [HttpGet("Sale")]
        public IActionResult Sale()
        {
            List<SaleDTO> model = new List<SaleDTO>();
            var entity = repoMenu.AllWithRating;
            var TotalOrderLines = repoOrderLine.All.Sum(x => x.Quantity);

            foreach (var item in entity)
            {
                var entityOrder = repoOrderLine.All.Where(x => x.MenuId == item.MenuId);
                var entityRate = repoRate.All.Where(x => x.OrderLine.MenuId == item.MenuId);
                model.Add(new SaleDTO
                {
                    MenuName = item.Name,
                    OrderCount = entityOrder.Count(),
                    OrderQuantity = entityOrder.Sum(x => x.Quantity),
                    Cash = entityOrder.Sum(x => x.Price * x.Quantity),
                    Rating = entity.Where(x => x.MenuId == item.MenuId).FirstOrDefault().Rating,
                    TotalProcent = (entityOrder.Sum(x => x.Quantity) / TotalOrderLines) * 100
                });

            }
            return Ok(model);
        }

        /// <summary>
        /// Pobranie danych do raportu pracowników
        /// </summary>
        /// <returns></returns>
        [HttpGet("Employee")]
        public IActionResult Employee()
        {
            List<EmployeeDTO> model = new List<EmployeeDTO>();
            var entity = repoUser.All.Where(x => x.UserRoles.FirstOrDefault().Role.Name == "Waiter");

            foreach (var item in entity)
            {
                var entityRateWaiter = repoWaiterRate.All.Where(x => x.WaiterID == item.Id);
                var AllWaiterRate = repoWaiterRate.All;
                var entityNotification =
               repoNotification.All.Where(x => x.UserAssigned == item.Id && x.CloseDate.Value.Year > 2000
                && x.CloseDate != null);
                if(entityRateWaiter.Count()>0)
                  {
                    var test =  entityRateWaiter.Average(x => x.Rating);
                 }
                model.Add(new EmployeeDTO
                {
                    FullName = item.FirstName + " " + item.LastName,
                    Rating = entityRateWaiter.Count() > 0 ? entityRateWaiter.Average(x => x.Rating) : 0,
                    WaiterAssignCalls = entityRateWaiter.Count(),
                    TotalCalls = AllWaiterRate.Count()



                });

            }
            return Ok(model);
        }


        /// <summary>
        /// Pobranie danych rocznej sprzedaży
        /// </summary>
        /// <returns></returns>
        [HttpGet("YearSale")]
        public IActionResult YearSale()
        {
            Random rand = new Random();
            var result = repoOrder.All.Where(x => x.CloseDate.Value.Year == DateTime.Now.Year-1).GroupBy(x => x.CloseDate.Value.Month)
                //.Select(x => new { Total = x.Sum(z => z.DocTotal), Name = x.Key })
                .Select(x => new { Total = rand.Next(5000) + 500, Name = x.Key })
                .ToList(); ;
     
      
        return Ok(result);
        }


    }
}