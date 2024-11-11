using System.Linq;
using MyRestaurant.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MyRestaurant.Data.Repositories
{
    public sealed class OrderRepository : GenericRepository<Order>
    {
        private readonly UserRepository _repoUser;
        public OrderRepository(DBContext context) : base(context)
        {

            _repoUser = new UserRepository(context);
        }

        public override IQueryable<Order> All
        {
            get { var result=  this.dbSet.Include(x=>x.OrderLines)/*.Include(x=>x.UserSign)*/;
                    foreach(var item in result)
                {
                    item.UserSign = _repoUser.All.Where(x => x.Id == item.UserSignId).FirstOrDefault();
                }
                return result;
                }
        }


    }

}
