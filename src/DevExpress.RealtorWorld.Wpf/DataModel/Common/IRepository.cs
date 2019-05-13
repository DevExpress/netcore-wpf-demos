using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DevExpress.RealtorWorld.Xpf.DataModel {
    public interface IRepository<TEntity, TPrimaryKey> where TEntity : class {
        IQueryable<TEntity> Get();
        TEntity Find(TPrimaryKey key);
        IUnitOfWork UnitOfWork { get; }
        ObservableCollection<TEntity> Local { get; }
    }
}
