using System;

namespace DevExpress.RealtorWorld.Xpf.DataModel {
    public interface IUnitOfWorkFactory {
        IRealtorWorldUnitOfWork CreateUnitOfWork();
    }
}