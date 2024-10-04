using Microsoft.EntityFrameworkCore;
using MyRestaurant.Data.Entities;
using System.Linq;

namespace MyRestaurant.Data.Repositories
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository(DBContext context) : base(context)
        { }

        public override IQueryable<User> All
        {
            get { return this.dbSet.Include(x => x.UserRoles).ThenInclude(ur => ur.Role); }
        }
    }
}
