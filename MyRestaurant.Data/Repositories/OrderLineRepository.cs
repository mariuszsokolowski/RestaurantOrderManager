using MyRestaurant.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyRestaurant.Data.Repositories
{
    public class OrderLineRepository : GenericRepository<OrderLine>
    {
        
        public OrderLineRepository(DBContext context) : base(context)
        {
        }

        public override IQueryable<OrderLine> All
        {
            get { return this.dbSet.Include(x=>x.Menu); }
        }
    }
}
