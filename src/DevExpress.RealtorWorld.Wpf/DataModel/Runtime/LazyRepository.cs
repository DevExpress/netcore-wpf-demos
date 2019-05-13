using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DevExpress.RealtorWorld.Xpf.DataModel {
    public abstract class LazyRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class {
        IQueryable<TEntity> IRepository<TEntity, TPrimaryKey>.Get() {
            return GetData().AsQueryable();
        }
        TEntity IRepository<TEntity, TPrimaryKey>.Find(TPrimaryKey key) {
            return FindCore(key);
        }
        ObservableCollection<TEntity> IRepository<TEntity, TPrimaryKey>.Local {
            get {
                return new ObservableCollection<TEntity>(GetData());
            }
        }
        IUnitOfWork IRepository<TEntity, TPrimaryKey>.UnitOfWork { get { return null; } }

        protected abstract IEnumerable<TEntity> GetData();
        protected abstract TEntity FindCore(TPrimaryKey id);
    }
}
