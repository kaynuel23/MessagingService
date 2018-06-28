using Fingrid.Messaging.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fingrid.Messaging.Data.Dapper
{
    public class GenericUnitOfWork : IDisposable
    {

        private string _tableName = string.Empty;

        public GenericUnitOfWork(string tableName)
        {
            _tableName = tableName;
        }

        public Dictionary<Type, object> repositories = new Dictionary<Type, object>();

        public IRepository<T> Repository<T>() where T : Entity
        {
            if (repositories.Keys.Contains(typeof(T)))
            {
                return repositories[typeof(T)] as IRepository<T>;
            }

            IRepository<T> dapperRepo = new DapperRepository<T>(_tableName);//For maint, let this passed from constructor
            repositories.Add(typeof(T), dapperRepo);
            return dapperRepo;

        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    new DapperRepository<IEntity>(_tableName).Connection.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
