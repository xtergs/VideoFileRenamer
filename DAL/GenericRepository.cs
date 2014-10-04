using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using VideoFileRenamer.UI;

namespace VideoFileRenamer.DAL
{
	internal class GenericRepository<T> where T : class
	{
		internal FilmContext context;
		internal DbSet<T> dbSet;

		public GenericRepository(FilmContext context)
		{
			this.context = context;
			this.dbSet = context.Set<T>();
		}

		public virtual IEnumerable<T> Get(Expression<Func<T, bool>> filter)
		{
			return dbSet.Where(filter);
		}

		public virtual void Add(T entity)
		{
			dbSet.Add(entity);
		}

		public virtual void Delete(object id)
		{
			T entityToDelete = dbSet.Find(id);
			Delete(entityToDelete);
		}

		public virtual void Delete(T entityToDelete)
		{
			if (context.Entry(entityToDelete).State == EntityState.Detached)
			{
				dbSet.Attach(entityToDelete);
			}
			dbSet.Remove(entityToDelete);
		}

		public virtual void Update(T entityToUpdate)
		{
			dbSet.Attach(entityToUpdate);
			context.Entry(entityToUpdate).State = EntityState.Modified;
		}
	}
}
