using Dapper;
using System.Data;
using System.Data.Common;
using System.Transactions;
using static Dapper.SqlMapper;

namespace TrackerApi.Core.Interfaces.Context
{
    public interface IUnitOfWork
    {
        DbConnection Connection { get; }
        DbTransaction Transaction { get; }

        #region Selectors

        Task<TEntity> QueryFirstOrDefaultAsync<TEntity>(string sqlQuery, object data, int? commandTimeout = null);
        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string sqlQuery, object? data = null, int? commandTimeout = null);
        #endregion
        #region Executors

        Task<int> ExecuteAsync(string sqlQuery, object? data = null, int? commandTimeout = null);
        Task<TEntity> ExecuteAsync<TEntity>(string sqlQuery, object? data = null, int? commandTimeout = null);
        Task<TEntity> ExecuteScalarAsync<TEntity>(string sqlQuery, object? data = null, int? commandTimeout = null);

        #endregion
        #region List

        IAsyncEnumerable<TReturn> ListYield<TFirst, TSecond, TReturn>(string sqlQuery, Func<TFirst, TSecond, TReturn> mapFunc, object? data = null, int? commandTimeout = null, CancellationToken token = default);

        Task<ValueTuple<object, object, object>> ListMultiple<TFirst, TSecond, TThird>(string sqlQuery, DynamicParameters dp);

        //Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> GetMultiple<T1, T2, T3>(string sql, IDictionary<string, object> dicParams,
        //                                Func<GridReader, IEnumerable<T1>> func1,
        //                                Func<GridReader, IEnumerable<T2>> func2,
        //                                Func<GridReader, IEnumerable<T3>> func3);

        ICollection<T> GetMultiple<T>(string sql, IDictionary<string, object> dicParams, params Func<GridReader, T>[] readerFuncs);

        #endregion
    }
}
