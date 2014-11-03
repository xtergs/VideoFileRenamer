using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace VideoFileRenamer.DAL
{
	internal class GenericRepository<T> : IRepository<T> where T : class
	{
		internal FilmContext context;
		internal DbSet<T> dbSet;

		public GenericRepository(FilmContext context)
		{
			this.context = context;
			this.dbSet = context.Set<T>();
		}

		public virtual IEnumerable<T> Get(Expression<Func<T, bool>> filter )
		{
			var result = dbSet.Where(filter);
			result.Load();
			return result;
		}

		public virtual void Add(T entity)
		{
			if (!IsContain(entity))
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

		public virtual bool IsContain(T entity)
		{
			return dbSet.Any(x => x == entity);
		}
	}
}
