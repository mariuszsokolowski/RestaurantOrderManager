using Microsoft.EntityFrameworkCore;
using MyRestaurant.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRestaurant.Data.Repositories
{
    public class OrderRateRepository : GenericRepository<OrderRate>
    {

        public OrderRateRepository(DBContext context) : base(context)
        { }
        public override  IQueryable<OrderRate> All
        {
            get { return this.dbSet.Include(x => x.OrderLine).Include(x => x.OrderLine.Menu); }
        }
    }
    
}
