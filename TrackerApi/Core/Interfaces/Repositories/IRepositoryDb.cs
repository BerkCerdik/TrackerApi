namespace TrackerApi.Core.Interfaces.Repositories
{
    public interface IRepositoryDb<TEntity> where TEntity : class
    {
        #region Single

        Task<TEntity> GetSingleAsync<TKey>(TKey id);
        Task<TEntity> GetSingleAsync(IDictionary<string, object> param);
        Task<TEntity> GetSingleAsync(string sqlQuery, object? param = null);

        #endregion

        #region Any

        Task<bool> GetAnyAsync<TKey>(TKey id);
        Task<bool> GetAnyAsync(IDictionary<string, object> param);
        Task<bool> GetAnyAsync(string sqlQuery, object? param = null);

        #endregion

        #region List

        Task<IEnumerable<TEntity>> GetListAsync();
        Task<IEnumerable<TEntity>> GetListAsync(IDictionary<string, object> param);
        Task<IEnumerable<TEntity>> GetListAsync(string sqlQuery, object? param = null);

        #endregion

        #region Insert

        Task<TEntity> InsertGetEntityAsync(TEntity entity);


        #endregion

        #region Update

        Task<TEntity> UpdateGetEntityAsync(IDictionary<string, object> param);

        #endregion

        #region Delete

        Task<int> DeleteAsync(Guid id);

        #endregion

    }
}
