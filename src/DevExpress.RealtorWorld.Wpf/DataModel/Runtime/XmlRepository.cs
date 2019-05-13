using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace DevExpress.RealtorWorld.Xpf.DataModel {
    public abstract class XmlRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class {
        static ObservableCollection<TEntity> dataSource;
        static object dataSourceLock = new object();

        IQueryable<TEntity> IRepository<TEntity, TPrimaryKey>.Get() { return DataSource.AsQueryable(); }
        ObservableCollection<TEntity> IRepository<TEntity, TPrimaryKey>.Local { get { return DataSource; } }
        IUnitOfWork IRepository<TEntity, TPrimaryKey>.UnitOfWork { get { return null; } }
        TEntity IRepository<TEntity, TPrimaryKey>.Find(TPrimaryKey id) {
            return DataSource.Where(o => object.Equals(GetKey(o), id)).FirstOrDefault();
        }
        ObservableCollection<TEntity> DataSource {
            get {
                lock(dataSourceLock) {
                    if(dataSource == null)
                        dataSource = GetNewDataSource();
                    return dataSource;
                }
            }
        }
        ObservableCollection<TEntity> GetNewDataSource() {
            using(ReusableStream data = GetDataStream()) {
                XmlSerializer s = new XmlSerializer(ObservableCollectionType);
                return (ObservableCollection<TEntity>)s.Deserialize(data.Data);
            }
        }

        protected abstract TPrimaryKey GetKey(TEntity entity);
        protected abstract ReusableStream GetDataStream();
        protected abstract Type ObservableCollectionType { get; }
        protected ReusableStream GetDataStreamCore(string file) {
            return DataHelper.GetDataFile(file);
        }
    }
}
