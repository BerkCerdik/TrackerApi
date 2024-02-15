using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using TrackerApi.Core.Interfaces.Context;
using static Dapper.SqlMapper;

namespace TrackerApi.Infrastructure.Context
{
    public class UnitOfWork : IUnitOfWork
    {
        public DbConnection Connection { get; private set; }
        public DbTransaction Transaction { get; private set; }

        public UnitOfWork(DbConnection connection)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        #region Sync

        #region Transaction

        public void Begin()
        {
            Transaction = Connection.BeginTransaction();
        }

        public void Begin(IsolationLevel isolationLevel)
        {
            Transaction = Connection.BeginTransaction(isolationLevel);
        }

        public void Commit()
        {
            Transaction?.Commit();
        }

        public void Rollback()
        {
            Transaction?.Rollback();
        }

        #endregion

        #endregion


        #region Async

        #region Transaction

        public async Task BeginAsync()
        {
            if (Connection.State != ConnectionState.Open)
            {
                if (Connection is SqlConnection sqlConnection)
                {
                    await sqlConnection.OpenAsync().ConfigureAwait(false);
                }
                else
                {
                    throw new InvalidOperationException("Async operations not supported for this IDbConnection type.");
                }
            }

            Transaction = Connection.BeginTransaction();
        }

        public async Task BeginAsync(IsolationLevel isolationLevel)
        {
            if (Connection.State != ConnectionState.Open)
            {
                if (Connection is SqlConnection sqlConnection)
                {
                    await sqlConnection.OpenAsync().ConfigureAwait(false);
                }
                else
                {
                    throw new InvalidOperationException("Async operations not supported for this IDbConnection type.");
                }
            }

            Transaction = Connection.BeginTransaction(isolationLevel);
        }

        public async Task CommitAsync()
        {
            if (Transaction is not null)
                await Task.Run(() => Transaction.Commit()).ConfigureAwait(false);
        }

        public async Task RollbackAsync()
        {
            if (Transaction is not null)
                await Task.Run(() => Transaction.Rollback()).ConfigureAwait(false);
        }

        #endregion

        #endregion
        #region Selectors
        public Task<TEntity> QueryFirstOrDefaultAsync<TEntity>(string sqlQuery, object? data = null, int? commandTimeout = null) => Connection.QueryFirstOrDefaultAsync<TEntity>(sqlQuery, data, commandTimeout: commandTimeout);

        public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string sqlQuery, object? data = null, int? commandTimeout = null) => Connection.QueryAsync<TEntity>(sqlQuery, data, commandTimeout: commandTimeout);
        #endregion
        #region Executors

        public Task<int> ExecuteAsync(string sqlQuery, object? data = null, int? commandTimeout = null) => Connection.ExecuteAsync(sqlQuery, data, commandTimeout: commandTimeout);

        public Task<TEntity> ExecuteAsync<TEntity>(string sqlQuery, object? data = null, int? commandTimeout = null) => Connection.QuerySingleAsync<TEntity>(sqlQuery, data, commandTimeout: commandTimeout);

        public Task<TEntity> ExecuteScalarAsync<TEntity>(string sqlQuery, object? data = null, int? commandTimeout = null) => Connection.QueryFirstOrDefaultAsync<TEntity>(sqlQuery, data, commandTimeout: commandTimeout);

        #endregion
        #region List

        public async IAsyncEnumerable<TReturn> ListYield<TFirst, TSecond, TReturn>(string sqlQuery, Func<TFirst, TSecond, TReturn> mapFunc, object? data = null, int? commandTimeout = null, CancellationToken token = default)
        {
            foreach (var item in Connection.Query(sqlQuery, mapFunc, data, buffered: false, commandTimeout: commandTimeout))
            {
                if (token.IsCancellationRequested)
                    yield break;

                yield return item;
            }
        }

        public async Task<ValueTuple<object, object, object>> ListMultiple<TFirst, TSecond, TThird>(string sqlQuery, DynamicParameters dp)
        {

            var data = Connection.QueryMultiple(sqlQuery, dp);

            var tFirstData = data.Read<TFirst>().FirstOrDefault();
            var tSecondData = data.Read<TSecond>().FirstOrDefault();
            var tThirdData = data.Read<TThird>().ToList();

            var tThirdListType = new List<TThird>();

            throw new NotImplementedException();
        }



        public ICollection<T> GetMultiple<T>
            (
            string sql,
            IDictionary<string, object> dicParams,
            params Func<GridReader, T>[] readerFuncs
            )
        {
            DynamicParameters dP = new();

            if (dicParams is not null)
            {
                foreach (KeyValuePair<string, object> item in dicParams)
                {
                    dP.Add(item.Key, item.Value);
                }
            }

            ICollection<T> values = new List<T>();
            var gridReader = Connection.QueryMultiple(sql, dP);

            foreach (var readerFunc in readerFuncs)
            {
                var obj = readerFunc(gridReader);
                values.Add(obj);
            }

            return values;
        }

        #endregion

    }
}
