using Microsoft.EntityFrameworkCore;
using RestaurantOrderManager.Data.Entities;
using System.Linq;

namespace RestaurantOrderManager.Data.Repositories
{
    public sealed class UserRepository : GenericRepository<User>
    {
        public UserRepository(DBContext context) : base(context)
        { }

        public override IQueryable<User> All
        {
            get { return this.dbSet.Include(x => x.UserRoles).ThenInclude(ur => ur.Role); }
        }
    }
}
