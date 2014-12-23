namespace VideoFileRenamer.DAL
{
	interface IRepository<T>  where T: class
	{
		T Add(T entity);

		void Delete(object id);

		void Delete(T entityToDelete);

		void Update(T entityToUpdate);

		bool IsContain(T entity);
	}
}
