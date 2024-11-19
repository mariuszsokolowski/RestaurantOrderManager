using Microsoft.EntityFrameworkCore;
using RestaurantOrderManager.Data.Entities;
using System.Linq;

namespace RestaurantOrderManager.Data.Repositories
{
    public sealed class MenuRepository : GenericRepository<Menu>
    {
        private readonly DBContext _context;
        private readonly DbSet<OrderRate> _orderRateEntity;
        public MenuRepository(DBContext context) : base(context)
        {
            _context = context;
            _orderRateEntity = _context.Set<OrderRate>();
        }

        public IQueryable<Menu> AllWithRating
        {

             get {
                          var result = this.dbSet;
                          foreach(var item in result)
                          {
                              var entity = _orderRateEntity.Where(x => x.OrderLine.MenuId == item.MenuId && x.Cancled==false && x.Rating>0);
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
