using Microsoft.EntityFrameworkCore;
using MyRestaurant.Data.Entities;
using System.Linq;

namespace MyRestaurant.Data.Repositories
{
    public class MenuRepository : GenericRepository<Menu>
    {
        private DBContext _context;
        private DbSet<OrderRate> OrderRate;
        public MenuRepository(DBContext context) : base(context)
        {
            _context = context;
            OrderRate = _context.Set<OrderRate>();
        }

        public virtual IQueryable<Menu> AllWithRating
        {

             get {
                          var result = this.dbSet;
                          foreach(var item in result)
                          {
                              var entity = OrderRate.Where(x => x.OrderLine.MenuId == item.MenuId && x.Cancled==false && x.Rating>0);
                              if (entity.Count() > 0)
                              { item.Rating = entity.Average(x => x.Rating); }
                              else
                              {
                                  item.Rating = 0;
                              }
                          }
                          return result;

            }
        }
    }
}
