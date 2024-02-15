using Dapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using TrackerApi.Core.Interfaces.Context;
using TrackerApi.Core.Interfaces.Repositories;

namespace TrackerApi.Infrastructure.Repository
{
    public class RepositoryDb<TEntity> : IRepositoryDb<TEntity> where TEntity : class
    {
        private readonly IUnitOfWork _unitOfWork;

        public RepositoryDb(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Single

        public async Task<TEntity> GetSingleAsync<TKey>(TKey id) => await _unitOfWork.QueryFirstOrDefaultAsync<TEntity>(GetSelectWithIdQuery(), new { Id = id });
        public async Task<TEntity> GetSingleAsync(IDictionary<string, object> param) => await _unitOfWork.QueryFirstOrDefaultAsync<TEntity>(GetSelectWithParamsQuery(param), param);
        public async Task<TEntity> GetSingleAsync(string sqlQuery, object? param = null) => await _unitOfWork.QueryFirstOrDefaultAsync<TEntity>(sqlQuery, param);

        #endregion

        #region Any

        public async Task<bool> GetAnyAsync<TKey>(TKey id) => await _unitOfWork.ExecuteScalarAsync<TEntity>(GetSelectWithIdQuery(), new { Id = id }) != null;
        public async Task<bool> GetAnyAsync(IDictionary<string, object> param) => await _unitOfWork.ExecuteScalarAsync<TEntity>(GetSelectWithParamsQuery(param), param) != null;
        public async Task<bool> GetAnyAsync(string sqlQuery, object? param = null) => await _unitOfWork.ExecuteScalarAsync<TEntity>(sqlQuery, param) != null;

        #endregion

        #region List

        public async Task<IEnumerable<TEntity>> GetListAsync() => await _unitOfWork.QueryAsync<TEntity>(GetSelectAllQuery());
        public async Task<IEnumerable<TEntity>> GetListAsync(IDictionary<string, object> param) => await _unitOfWork.QueryAsync<TEntity>(GetSelectWithParamsQuery(param), param);
        public async Task<IEnumerable<TEntity>> GetListAsync(string sqlQuery, object? param = null) => await _unitOfWork.QueryAsync<TEntity>(sqlQuery, param);

        #endregion

        #region Insert

        public async Task<TEntity> InsertGetEntityAsync(TEntity entity)
        {
            (string query, DynamicParameters @params) insertQuery = InsertQuery(entity, returnOutput: true, getAll: true);
            return await _unitOfWork.ExecuteAsync<TEntity>(insertQuery.query, insertQuery.@params, null);
        }

        #endregion

        #region Update

       

        public async Task<TEntity> UpdateGetEntityAsync(IDictionary<string, object> param)
        {
            (string query, DynamicParameters @params) updateQuery = UpdateQuery(param, returnOutput: true, getAll: true);
            return await _unitOfWork.ExecuteAsync<TEntity>(updateQuery.query, updateQuery.@params);
        }


        #endregion

        #region Delete

        public async Task<int> DeleteAsync(Guid id)
        {
            (string query, DynamicParameters @params) deleteQuery = DeleteQuery(id, returnOutput: false);
            return await _unitOfWork.ExecuteAsync(deleteQuery.query, deleteQuery.@params);
        }
       
        #endregion

        #region Helpers

        private string GetTableNameOfEntity()
        {
            Type entityType = typeof(TEntity);
            return entityType.GetCustomAttribute<TableAttribute>()?.Name ?? entityType.Name;
        }
        private object? GetPropertyValue(TEntity entity, string propName) => typeof(TEntity).GetProperty(propName)?.GetValue(entity);

        private List<string> GetEntityColumnNames(Type entityType)
        {
            List<PropertyInfo> entityProps = entityType.GetProperties().ToList();
            entityProps.RemoveAll(x => x.DeclaringType == typeof(TEntity) && (x.GetGetMethod()?.IsVirtual ?? true));

            return entityProps.Select(x => x.Name).ToList();
        }

        private string GetSelectAllQuery() => $"SELECT * FROM [{GetTableNameOfEntity()}]";

        private string GetSelectWithIdQuery() => $"SELECT * FROM [{GetTableNameOfEntity()}] WHERE [Id] = @Id";

        private string GetSelectWithParamsQuery(IDictionary<string, object> paramsDic)
        {
            var query = $"SELECT * FROM [{GetTableNameOfEntity()}] WHERE 1=1 ";
            foreach (var item in paramsDic) query += $"AND [{item.Key}] = @{item.Key} ";
            return query;
        }

        private (string query, DynamicParameters @params) InsertQuery(TEntity entity, bool returnOutput = true, bool getAll = true)
        {
            DynamicParameters dynamicParams = new DynamicParameters();

            string sqlQuery = $"INSERT INTO [dbo].[{GetTableNameOfEntity()}] ([";

            Type entityType = entity.GetType();

            List<string> entityPropNames = GetEntityColumnNames(entityType);
            sqlQuery += string.Join("],[", entityPropNames) + "]";

            if (returnOutput)
                sqlQuery += $") OUTPUT Inserted.{(getAll ? "*" : "Id")} VALUES (";
            else
                sqlQuery += ") VALUES (";


            foreach (string entityPropertyName in entityPropNames)
            {
                object? entityValue = GetPropertyValue(entity, entityPropertyName);

                sqlQuery += $"@{entityPropertyName}, ";
                dynamicParams.Add($"@{entityPropertyName}", entityValue);
            }

            sqlQuery = sqlQuery.TrimEnd(' ', ',');

            return (sqlQuery + ")", dynamicParams);
        }

        private (string query, DynamicParameters @params) UpdateQuery(IDictionary<string, object> param, bool returnOutput = true, bool getAll = true)
        {
            DynamicParameters dynamicParams = new DynamicParameters();

            string sqlQuery = $"UPDATE [dbo].[{GetTableNameOfEntity()}] SET ";

            foreach (var item in param.Where(x => x.Key != "Id"))
            {
                sqlQuery += $"[{item.Key}] = @{item.Key},";
                dynamicParams.Add($"@{item.Key}", item.Value);
            }

            sqlQuery = sqlQuery.TrimEnd(',', ' ');

            if (returnOutput)
                sqlQuery += $" OUTPUT Inserted.{(getAll ? "*" : "Id")} ";
            sqlQuery += " WHERE [Id] = @Id ";

            dynamicParams.Add("Id", param.FirstOrDefault(x => x.Key == "Id").Value);

            return (sqlQuery, dynamicParams);
        }

        private (string query, DynamicParameters @params) DeleteQuery(Guid Id, bool returnOutput = true, bool getAll = true)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();

            string sqlQuery = $"DELETE FROM [dbo].[{GetTableNameOfEntity()}]";

            if (returnOutput)
                sqlQuery += $" OUTPUT Deleted.{(getAll ? "*" : "Id")}";
            sqlQuery += " WHERE [Id] = @Id ";

            dynamicParameters.Add("Id", Id);

            return (sqlQuery, dynamicParameters);
        }


        #endregion
    }
}
