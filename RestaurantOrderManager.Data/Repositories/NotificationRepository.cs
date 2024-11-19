using RestaurantOrderManager.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrderManager.Data.Repositories
{
    public sealed class NotificationRepository : GenericRepository<Notification>
    {
        private readonly UserRepository _repoUser;
        public NotificationRepository(DBContext context) : base(context)
        {
            _repoUser = new UserRepository(context);
        }

        public override IQueryable<Notification> All
        {
            get
            {
                var result = this.dbSet;
                foreach (var item in result)
                {
                    item.UserSign = _repoUser.All.Where(x => x.Id == item.UserSignId).FirstOrDefault();
                }
                return result;

            }
        }
    }

}

