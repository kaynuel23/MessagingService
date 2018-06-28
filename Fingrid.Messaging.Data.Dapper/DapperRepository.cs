using Dapper;
using Fingrid.Messaging.Core;
using RepoWrapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fingrid.Messaging.Data.Dapper
{
    public class DapperRepository<T> : IRepository<T> where T : class, IEntity
    {
        protected readonly string _tableName;
        private readonly string connectionString = "";
        public IDbConnection Connection
        {
            get
            {
                var factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
                var conn = factory.CreateConnection();
                conn.ConnectionString = connectionString;
                conn.OpenAsync();
                return conn;
            }
        }
        public DapperRepository(string tableName)
        {
            _tableName = tableName;
            connectionString = ConfigurationManager.ConnectionStrings["Fingrid.Messaging"].ConnectionString;
        }
        protected async Task<T> WithConnection<T>(Func<IDbConnection, Task<T>> getData)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync(); // Asynchronously open a connection to the database
                    return await getData(connection); // Asynchronously execute getData, which has been passed in as a Func<IDBConnection, Task<T>>
                }
            }
            catch (TimeoutException ex)
            {
                throw new Exception(String.Format("{0}.WithConnection() experienced a SQL timeout", GetType().FullName), ex);
            }
            catch (SqlException ex)
            {
                throw new Exception(String.Format("{0}.WithConnection() experienced a SQL exception (not a timeout)", GetType().FullName), ex);
            }
        }
        internal virtual dynamic Mapping(T item)
        {
            return item;
        }

        public virtual async Task<int> DeleteAsync(T entity)
        {
            //using (IDbConnection conn = Connection)
            //{
            //conn.Open();
            //SqlMapper.Execute(Connection, "DELETE FROM " + _tableName + " WHERE ID=@ID", new { ID = entity.ID });
            //}
            int rowsAffected = 0;
            await WithConnection(async c => {
                using (var transaction = c.BeginTransaction())
                {
                    rowsAffected = await c.ExecuteAsync("DELETE FROM " + _tableName + " WHERE ID=@ID", new { ID = entity.ID });
                    transaction.Commit();
                    return rowsAffected;
                }
            });
            return rowsAffected;
        }

        public virtual async Task DeleteByIdAsync(long id)
        {
            T entity = await GetByIdAsync(id);
            await DeleteAsync(entity);
        }


        public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {

            // extract the dynamic sql query and parameters from predicate
            if (filter != null)
            {
                QueryResult result = DynamicQuery.GetDynamicQuery(_tableName, filter);
                return await WithConnection(async c => {
                    var results = await c.QueryAsync<T>(result.Sql, (object)result.Param);
                    return results;
                });
            }
            else
            {
                return await WithConnection(async c => {
                    var results = await c.QueryAsync<T>("SELECT * FROM " + _tableName);
                    return results;
                });
            }
        }

        public virtual async Task<IEnumerable<T>> GetTopNAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "", int noOfItems = 0)
        {

            // extract the dynamic sql query and parameters from predicate
            if (filter != null)
            {
                QueryResult result = DynamicQuery.GetTopNDynamicQuery(_tableName, filter, noOfItems);
                return await WithConnection(async c => {
                    var results = await c.QueryAsync<T>(result.Sql, (object)result.Param);
                    return results;
                });
            }
            else
            {
                return await WithConnection(async c => {
                    var results = await c.QueryAsync<T>("SELECT * FROM " + _tableName);
                    return results;
                });
            }
        }

        public virtual async Task<T> GetByIdAsync(long id)
        {
            return await WithConnection(async c => {

                return await c.QueryFirstOrDefaultAsync<T>("SELECT * FROM " + _tableName + " WHERE ID=@ID", new { ID = id });

            });
        }

        private static string BuildInsertValues(T entity)
        {
            StringBuilder sb = new StringBuilder();
            var props = typeof(T).GetProperties().ToArray();
            for (var i = 0; i < props.Count(); i++)
            {
                var property = props.ElementAt(i);
                if (property.Name.ToUpper() == "ID")
                    continue;
                //sb.AppendFormat($"@{property.Name}");
                sb.AppendFormat("@{0}", property.Name);
                if (i < props.Count() - 1)
                    sb.Append(", ");
            }
            if (sb.ToString().EndsWith(", "))
                sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }

        public virtual async Task<int> InsertAsync(T entity)
        {
            return await WithConnection(async c => {
                var parameters = (object)Mapping(entity);
                string insertQuery = DynamicQuery.GetInsertQuery(_tableName, parameters);
                //using (var transaction = c.BeginTransaction())
                {
                    //conn.Open();
                    IEnumerable<int> result = await c.QueryAsync<int>(insertQuery);
                    //transaction.Commit();
                    return result.SingleOrDefault();
                }
            });
        }

        public virtual async Task<int> UpdateAsync(T entity)
        {
            return await WithConnection(async c => {
                var parameters = (object)Mapping(entity);
                string updateQuery = DynamicQuery.GetUpdateQueryForIdentity(_tableName, parameters);
                //using (var transaction = c.BeginTransaction())
                {
                    //transaction.Commit();
                    return await c.ExecuteAsync(updateQuery, entity);
                }
            });
        }
    }
}
