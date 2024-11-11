using Microsoft.EntityFrameworkCore;
using MyRestaurant.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRestaurant.Data.Repositories
{
    public sealed class WaiterRateRepository : GenericRepository<WaiterRate>
    {
        private readonly UserRepository _repoUser;
        public WaiterRateRepository(DBContext context) : base(context)
        {
            _repoUser = new UserRepository(context);
        }
        public override IQueryable<WaiterRate> All
        {
            get
            {
                var result = this.dbSet;
                foreach (var item in result)
                {
                    item.Waiter = _repoUser.All.Where(x => x.Id == item.WaiterID).FirstOrDefault();
                }
                return result;

            }

        }

    }
}
