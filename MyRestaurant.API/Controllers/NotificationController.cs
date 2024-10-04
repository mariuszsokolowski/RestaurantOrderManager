using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Storage;
using MyRestaurant.API.SignalR;
using MyRestaurant.Data;
using MyRestaurant.Data.Entities;
using MyRestaurant.Data.Repositories;

namespace MyRestaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
         
        private readonly DBContext _context;
        private NotificationRepository repoNotification;
        private WaiterRateRepository repoWaiterRate;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;
        private IHubContext<RateHub> rateHub;
        private IHubContext<NotificationHub> notificationHub;


        public NotificationController(DBContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IHubContext<RateHub> _rateHub, IHubContext<NotificationHub> _notificationHub)
        {
            notificationHub = _notificationHub;
            rateHub = _rateHub;
            _context = context;
            _env = env;
            repoNotification = new NotificationRepository(context);
            repoWaiterRate = new WaiterRateRepository(context);
        }

        /// <summary>
        /// Pobranie zgłoszeń
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public IActionResult Get()
        {

            var userID = this.User.FindFirstValue(ClaimTypes.NameIdentifier);


            var result = repoNotification.All.Where(x=>x.CloseDate==null && (x.UserAssigned==userID || x.UserAssigned==null));
            if (result.Count() <= 0)
                return NoContent();

            return Ok(result);
        }

        /// <summary>
        /// Przywołanie kelnera
        /// </summary>
        /// <returns></returns>
        [HttpPost("CallWaiter")]
        public IActionResult CallWaiter()
        {
            try
            {
                var userID = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userID == null)
                    return NoContent();

                var entity = new Notification()
                {
                    CreateDate = DateTime.Now,
                    UserSignId = userID
                };
                repoNotification.Insert(entity);
                repoNotification.Save();
                SendNewNotificationInfo();
                return Ok("Kelner został wezwany.");
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }

        }


        /// <summary>
        /// Przypisanie do zgłoszenia
        /// </summary>
        /// <returns></returns>
        [HttpPost("Assign")]
        public IActionResult Assign([FromBody] NotificationModel model)
        {
            try
            {
                var userID = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userID == null)
                    return NoContent();

                var entity = repoNotification.All.Where(x => x.NotificationID == model.NotificationID).FirstOrDefault();
                if (entity == null)
                    return NoContent();

                if (entity.UserAssigned != null)
                    return NotFound("Zgłoszenie zostało już przypisane");

                entity.AssignedDate = DateTime.Now;
                entity.UserAssigned = userID;
                repoNotification.Update(entity);
                repoNotification.Save();
                return Ok("Zgłoszenie nr " + entity.NotificationID +" zostało przypisane.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }


        /// <summary>
        /// Zakończenie zgłoszenia
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Done")]
        public IActionResult Done([FromBody] NotificationModel model)
        {
            try
            {
                var userID = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userID == null)
                    return NoContent();

                using (IDbContextTransaction dbTransaction = _context.Database.BeginTransaction())
                {

                    try
                    {
                        var entity = repoNotification.All.Where(x => x.NotificationID == model.NotificationID).FirstOrDefault();
                        WaiterRate entityRate = new WaiterRate()
                        {
                            CreateDate = DateTime.Now,
                        NotificationID = model.NotificationID,
                        Type = Data.Enums.RatingType.Waiter,
                        UserSign = entity.UserSignId,
                        WaiterID = entity.UserAssigned
                          };
                        repoWaiterRate.Insert(entityRate);
                        repoWaiterRate.Save();
                        
                        if (entity == null)
                            return NoContent();

                        entity.CloseDate = DateTime.Now;
                        repoNotification.Update(entity);
                        repoNotification.Save();
                        
                        dbTransaction.Commit();
                        SendWaiterRateInfo();
                        return Ok("Zgłoszenie nr: " + model.NotificationID + " zostało zamknięte.");
                    }

                    catch (Exception e)
                    {
                        dbTransaction.Rollback();
                        return NotFound(e.Message);
                    }

                }
                    
            
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        public class NotificationModel
        {
            public int NotificationID { get; set; }
            public int Rating { get; set; }
        }

        private void SendWaiterRateInfo()
        {
            rateHub.Clients.All.SendAsync("WaiterRate");
        }
        private void SendNewNotificationInfo()
        {
            notificationHub.Clients.All.SendAsync("NewNotification");
        }
    }
}