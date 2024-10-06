using Bogus;
using MyRestaurant.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyRestaurant.API.Fakers
{
    public class Faker : IDisposable
    {
        public Menu MenuFaker()
        {
            var faker = new Faker<Menu>()
                     .RuleFor(o => o.Name, f => f.Commerce.ProductName())
                    .RuleFor(o => o.Description, f => f.Commerce.ProductDescription())
                    .RuleFor(o => o.Rating, f => f.Random.Int(1, 5)+f.Random.Decimal())
                    .RuleFor(o => o.Price, f => f.Random.Int(1, 100000) + f.Random.Float())
                    //ToDo: Image
                    .RuleFor(o => o.Promotion, f => f.Random.Bool())
                    .RuleFor(o => o.Inactive, f => f.Random.Bool());
            return faker.Generate();
        }

        public Notification NotificationFaker()
        {

            var faker = new Faker<Notification>()
                    //ToDo UserSignId,UserSign,UserAssigned
                    .RuleFor(o => o.CreateDate, f => f.Date.Past())
                    .RuleFor(o => o.AssignedDate, f => f.Date.Past())
                    .RuleFor(o => o.CloseDate, f => f.Date.Past());

            return faker.Generate();
        }
        public Notification OrderFaker()
        {

            var faker = new Faker<Notification>()
                    //ToDo UserSignId,UserSign,UserAssigned
                    .RuleFor(o => o.CreateDate, f => f.Date.Past())
                    .RuleFor(o => o.AssignedDate, f => f.Date.Past())
                    .RuleFor(o => o.CloseDate, f => f.Date.Past());

            return faker.Generate();
        }

        public void Dispose()
        {
        }
    }
}
