using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MyRestaurant.Data.Repositories
{
    public abstract class GenericRepository<T> : IDisposable where T : class
    {
        private readonly DBContext context;
        protected DbSet<T> dbSet { get; set; }

        protected GenericRepository(DBContext _context)
        {
            context = _context;
            this.dbSet = context.Set<T>();
        }
        public virtual IQueryable<T> All
        {
            get { return this.dbSet; }
        }

        public virtual T FindById(int id)
        {
            return this.dbSet.Find(id);
        }

        public virtual void Insert(T entity)
        {
            this.dbSet.Add(entity);
        }

        public virtual void Update(T entity)
        {
            this.context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(int id)
        {
            var obj = this.dbSet.Find(id);
            this.dbSet.Remove(obj);
        }

        public virtual void Save()
        {
            this.context.SaveChanges();
        }

        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}
