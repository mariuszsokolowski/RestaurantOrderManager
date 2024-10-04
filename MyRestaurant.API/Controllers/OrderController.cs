using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Storage;
using MyRestaurant.API.Filters;
using MyRestaurant.API.Models;
using MyRestaurant.API.SignalR;
using MyRestaurant.Data;
using MyRestaurant.Data.Entities;
using MyRestaurant.Data.Repositories;
using Newtonsoft.Json;



namespace MyRestaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly DBContext _context;
        private OrderRepository repoOrder;
        private OrderRateRepository repoRate;
        private OrderLineRepository repoOrderLine;
        private UserRepository repoUser;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;
        private readonly IMapper mapper;
        private MenuRepository repoMenu;
        private IHubContext<RateHub> rateHub;
        private IHubContext<OrderHub> orderHub;


        public OrderController(DBContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IMapper _mapper, IHubContext<RateHub> _rateHub, IHubContext<OrderHub> _orderHub)
        {

            rateHub = _rateHub;
            orderHub = _orderHub;
            _env = env;
            _context = context;

            mapper = _mapper;

            #region AddRepository
            repoUser = new UserRepository(_context);
            repoOrder = new OrderRepository(_context);
            repoOrderLine = new OrderLineRepository(_context);
            repoMenu = new MenuRepository(_context);
            repoRate = new OrderRateRepository(_context);
            #endregion



        }

        /// <summary>
        /// Pobranie zamówień
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public IActionResult Get()
        {

            var result = repoOrder.All;
            if (result.Count() <= 0)
                return NoContent();

            return Ok(result);
        }

        /// <summary>
        /// Pobranie uruchomioncyh zamówień (zamówienia dla kucharaza)
        /// </summary>
        /// <returns></returns>
        [HttpGet("Running")]
        public IActionResult GetRunnig()
        {
  
            var result = repoOrder.All.Where(x => x.CloseDate == null);
            if (result.Count() <= 0)
                return NoContent();



            //var model = mapper.Map<List<Order>, List<OrderModels>>(result.ToList());
            //Tutaj trzeba ogarnąć lazy loading
            foreach (var item in result)
            {

                item.OrderLines = repoOrderLine.All.Where(x => x.OrderId == item.OrderId).ToList();
            }

            return Ok(result);
        }

        /// <summary>
        /// Pobranie zamówień do wydania (zamówienia dla kelnera)
        /// </summary>
        /// <returns></returns>
        [HttpGet("Completed")]
        public IActionResult GetCompleted()
        {

            var result = repoOrder.All.Where(x => x.CloseDate != null && x.Close == false);
            if (result.Count() <= 0)
                return NoContent();

            return Ok(result);
        }

        /// <summary>
        /// Pobranie zamówienia po identyfikatorze
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var result = repoOrder.FindById(id);

            if (result != null)
                return NotFound(id);

            return Ok();
        }


        /// <summary>
        /// Zakończenie zamówienia
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Done")]
        public IActionResult Done([FromBody] OrderModels model)
        {
            try
            {
                var entity = repoOrder.FindById(model.OrderId);

                if (entity == null)
                    return NotFound(model.OrderId);


                entity.CloseDate = DateTime.Now;
                repoOrder.Update(entity);
                repoOrder.Save();
                SendDoneOrderInfo();

                return Ok("Zamówienie nr: " + model.OrderId + " zostało zakończone");

            }
            catch
            {
                return NotFound(model.OrderId);
            }



        }

        /// <summary>
        /// Zakończenie zamówienia przez kucharza
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Close")]
        public IActionResult Close([FromBody] OrderModels model)
        {
            try
            {
                var entity = repoOrder.FindById(model.OrderId);

                if (entity == null)
                    return NotFound(model.OrderId);


                using (IDbContextTransaction dbTransaction = _context.Database.BeginTransaction())
                {

                    try
                    {
                        entity.Close = true;
                        repoOrder.Update(entity);

                        //Insert rate entity
                        foreach (var item in model.OrderLines)
                        {

                            OrderRate entityRate = new OrderRate()
                            {
                                CreateDate = DateTime.Now,
                                OrderLineId = item.OrderLineId,
                                Type = Data.Enums.RatingType.Menu,
                                UserSign = entity.UserSignId
                               
                            };
                            repoRate.Insert(entityRate);

                        }
                        repoRate.Save();
                        repoOrder.Save();
                        dbTransaction.Commit();
                        SendMenuRateInfo();

                    }
                    catch
                    {
                        dbTransaction.Rollback();
                        return NotFound(model.OrderId);
                    }

                }

               
                return Ok("Zamówienie nr: " + model.OrderId + " zostało zamknięte");

            }
            catch
            {
                return NotFound(model.OrderId);
            }





        }

        /// <summary>
        /// Dodanie zamówienia
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("")]
        public IActionResult Post([FromBody] List<OrderLineModels> model)

        {


            if (!ModelState.IsValid || model[0].PaymentMethod <= 0)
            {
                return BadRequest();
            }
            if (model.Count <= 0)
            {
                return BadRequest();
            }


            using (IDbContextTransaction dbTransaction = _context.Database.BeginTransaction())
            {
                try
                {


                    Order eOrder = new Order()
                    {
                        CreateDate = DateTime.Now,
                        DocTotal = (float)model.Sum(x => x.Price * x.Quantity),

                        UserSignId = this.User.FindFirstValue(ClaimTypes.NameIdentifier),


                        PaymentMethod = model[0].PaymentMethod
                    };
                    repoOrder.Insert(eOrder);
                    repoOrder.Save();

                    List<OrderLine> eOrderLine = new List<OrderLine>();

                    foreach (var item in model)
                    {
                        item.Menu = repoMenu.FindById(item.MenuId);
                        item.Order = eOrder;
                        var eOrderLineOneLine = mapper.Map<OrderLineModels, OrderLine>(item);
                        eOrderLineOneLine.OrderId = eOrder.OrderId;
                        eOrderLine.Add(eOrderLineOneLine);
                    }

                    foreach (var item in eOrderLine)
                    {
                        repoOrderLine.Insert(item);
                    }





                    repoOrderLine.Save();
                    dbTransaction.Commit();
                    SendNewOrderInfo();
                    return Ok("Zamówienie nr: " + eOrder.OrderId + " zostało przyjęte do realizacji.");
                }

                catch (Exception e)
                {
                    dbTransaction.Rollback();
                    return NotFound(e.Message);
                }

            }
        }


        /// <summary>
        /// Dodanie zamówienia za klienta
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("ForClient")]
        public IActionResult ForClient([FromBody] List<OrderLineModels> model)

        {


            if (!ModelState.IsValid || model[0].PaymentMethod <= 0 && (model[0].UserSign==null || model[0].UserSign.Length<=0))
            {
                return BadRequest();
            }
            if (model.Count <= 0)
            {
                return BadRequest();
            }

            var user = repoUser.All.Where(x => x.UserName.ToUpper() == model[0].UserSign.ToUpper()).FirstOrDefault();
            if(user==null)
            {
                return BadRequest();
            }
            using (IDbContextTransaction dbTransaction = _context.Database.BeginTransaction())
            {
                try
                {


                    Order eOrder = new Order()
                    {
                        CreateDate = DateTime.Now,
                        DocTotal = (float)model.Sum(x => x.Price * x.Quantity),

                        UserSign = user,


                        PaymentMethod = model[0].PaymentMethod
                    };
                    repoOrder.Insert(eOrder);
                    repoOrder.Save();

                    List<OrderLine> eOrderLine = new List<OrderLine>();

                    foreach (var item in model)
                    {
                        item.Menu = repoMenu.FindById(item.MenuId);
                        item.Order = eOrder;
                        var eOrderLineOneLine = mapper.Map<OrderLineModels, OrderLine>(item);
                        eOrderLineOneLine.OrderId = eOrder.OrderId;
                        eOrderLine.Add(eOrderLineOneLine);
                    }

                    foreach (var item in eOrderLine)
                    {
                        repoOrderLine.Insert(item);
                    }





                    repoOrderLine.Save();
                    dbTransaction.Commit();
                    SendNewOrderInfo();
                    return Ok("Zamówienie nr: " + eOrder.OrderId + " zostało przyjęte do realizacji.");
                }

                catch (Exception e)
                {
                    dbTransaction.Rollback();
                    return NotFound(e.Message);
                }

            }
        }

        /// <summary>
        /// Aktualziacja zamówienia
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        [HttpPut("")]
        public IActionResult Put(Order model)
        {

            if (!ModelState.IsValid || model.OrderId <= 0)
                return BadRequest(model);


            try
            {

                repoOrder.Update(model);
                repoOrder.Save();

                return Ok(model);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

        }
        private void SendMenuRateInfo()
        {
            rateHub.Clients.All.SendAsync("MenuRate");
        }


        private void SendNewOrderInfo()
        {
            orderHub.Clients.All.SendAsync("NewOrder");
        }
        private void SendDoneOrderInfo()
        {
            orderHub.Clients.All.SendAsync("DoneOrder");
        }

    }
}