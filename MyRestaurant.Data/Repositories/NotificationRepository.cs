using MyRestaurant.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRestaurant.Data.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>
    {
        UserRepository repoUser;
        public NotificationRepository(DBContext context) : base(context)
        {
            repoUser = new UserRepository(context);
        }

        public override IQueryable<Notification> All
        {
            get
            {
                var result = this.dbSet;
                foreach (var item in result)
                {
                    item.UserSign = repoUser.All.Where(x => x.Id == item.UserSignId).FirstOrDefault();
                }
                return result;

            }
        }
    }

}

