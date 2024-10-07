using Bogus;
using MyRestaurant.Data;
using MyRestaurant.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyRestaurant.API.Fakers
{
    public class Faker : IDisposable
    {
        #region Fields
        private DBContext _context;
        private Random _rand;
        #endregion

        #region Constructors
        public Faker(DBContext context)
        {
            _rand = new Random();
            _context = context;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Create fake Menu entity
        /// </summary>
        /// <returns></returns>
        public Menu MenuFaker()
        {
            var faker = new Faker<Menu>()
                     .RuleFor(o => o.Name, f => f.Commerce.ProductName())
                    .RuleFor(o => o.Description, f => f.Commerce.ProductDescription())
                    .RuleFor(o => o.Rating, f => f.Random.Int(1, 5) + f.Random.Decimal())
                    .RuleFor(o => o.Price, f => f.Random.Int(1, 100000) )
                    .RuleFor(o=>o.Image, f=>f.Image.PicsumUrl())
                    .RuleFor(o => o.Promotion, f => f.Random.Bool())
                    .RuleFor(o => o.Inactive, f => f.Random.Bool());
            return faker.Generate();
        }

        /// <summary>
        /// Create fake Notification entity
        /// </summary>
        /// <returns></returns>
        public Notification NotificationFaker()
        {
            List<User> userList = _context.Set<User>().ToList();
            int userIndex = _rand.Next(0, userList.Count() - 1);
            var faker = new Faker<Notification>()
                    //ToDo UserAssigned
                    .RuleFor(o => o.CreateDate, f => f.Date.Past())
                    .RuleFor(o => o.AssignedDate, f => f.Date.Past())
                    .RuleFor(o => o.CloseDate, f => f.Date.Past())
                    .RuleFor(o => o.UserSignId, userList[userIndex].Id);

            return faker.Generate();
        }

        #region Orders
        /// <summary>
        /// Create fake Order entity
        /// </summary>
        /// <returns></returns>
        public Order OrderFaker()
        {
            var isClosed = _rand.Next(0, 1) == 1;
            bool isCancel = false;
            if (!isClosed)
                isCancel = _rand.Next(0, 1) == 1;

            List<User> userList = GetUserList();
            int userIndex = _rand.Next(0, userList.Count() - 1);

            var faker = new Faker<Order>()
                    //ToDo UserAssigned
                    .RuleFor(o => o.DocTotal, f => f.Random.Int(1, 99999))
                    .RuleFor(o => o.PaymentMethod, f => Data.Enums.PaymentMethod.Gotówka)
                    .RuleFor(o => o.Cancel, isCancel)
                    .RuleFor(o => o.CreateDate, f => DateTime.Now)
                    .RuleFor(o => o.Close, isClosed)
                    .RuleFor(o => o.UserSignId, userList[userIndex].Id);
            if (isClosed)
                faker
                .RuleFor(o => o.CloseDate, f => f.Date.Past());
            return faker.Generate();
        }
        /// <summary>
        /// Create fake OrderLine entity
        /// </summary>
        /// <returns></returns>
        public OrderLine OrderLineFaker(int orderId)
        {
            List<Menu> menuList = _context.Set<Menu>().ToList();
            int menuIndex = _rand.Next(0, menuList.Count() - 1);

            var faker = new Faker<OrderLine>()
                    .RuleFor(o => o.OrderId, f => orderId)
                    .RuleFor(o => o.Price, f => Convert.ToDouble(f.Commerce.Price()))
                    .RuleFor(o => o.Quantity, f => f.Random.Int(10, 999))
                    .RuleFor(o => o.MenuId, menuList[menuIndex].MenuId);
            return faker.Generate();
        }
        #endregion

        /// <summary>
        /// Create fake OrderRate entity
        /// </summary>
        /// <returns></returns>
        public OrderRate OrderRateFaker(int orderLineId)
        {
            List<User> userList = GetUserList();
            int userIndex = _rand.Next(0, userList.Count() - 1);

            var faker = new Faker<OrderRate>()
                    .RuleFor(o => o.Type, f => Data.Enums.RatingType.Menu)
                    .RuleFor(o => o.OrderLineId, orderLineId)
                    .RuleFor(o => o.CreateDate, f => f.Date.Past())
                    .RuleFor(o => o.Rating, f => f.Random.Int(1, 5))
                    .RuleFor(o => o.Cancled, f => f.Random.Bool())
                     .RuleFor(o => o.UserSign, userList[userIndex].Id);

            return faker.Generate();
        }

        /// <summary>
        /// Create fake WaiterRate entity
        /// </summary>
        /// <returns></returns>
        public WaiterRate WaiterRateFaker()
        {
            List<Notification> notificationList = _context.Set<Notification>().ToList();
            int notificationIndex = _rand.Next(0, notificationList.Count() - 1);

            List<User> userList = GetUserList();
            int userIndex = _rand.Next(0, userList.Count() - 1);

            List<User> waiterList = _context.Set<User>().ToList();
            int waiterIndex = _rand.Next(0, waiterList.Count() - 1);
            var faker = new Faker<WaiterRate>()
                        .RuleFor(o => o.WaiterID, f => waiterList[waiterIndex].Id)
                        .RuleFor(o => o.Type, f => Data.Enums.RatingType.Waiter)
                        .RuleFor(o => o.NotificationID, notificationList[notificationIndex].NotificationID)
                        .RuleFor(o => o.CreateDate, f => f.Date.Past())
                        .RuleFor(o => o.Rating, f => f.Random.Int(1, 5))
                        .RuleFor(o => o.Cancled, f => f.Random.Bool())
                         .RuleFor(o => o.UserSign, userList[userIndex].Id);
            return faker.Generate();
        }
        /// <summary>
        /// Get user list
        /// </summary>
        /// <param name="role">Add param to get users with Role</param>
        /// <returns></returns>
        private List<User> GetUserList(string role ="")
        {
            List<User> result = _context.Set<User>().Where(x =>String.IsNullOrEmpty(role) || x.UserRoles.Any(u => u.Role.Name == role )).ToList();
            return result;
        }


        public void Dispose()
        {
        }

        #endregion
    }
}
