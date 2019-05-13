using DevExpress.DevAV;
using DevExpress.Mvvm.DataModel;
using DevExpress.Mvvm.DataModel.DesignTime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevExpress.DevAV.DevAVDbDataModel {

    /// <summary>
    /// A DevAVDbDesignTimeUnitOfWork instance that represents the design-time implementation of the IDevAVDbUnitOfWork interface.
    /// </summary>
    public class DevAVDbDesignTimeUnitOfWork : DesignTimeUnitOfWork, IDevAVDbUnitOfWork {

        /// <summary>
        /// Initializes a new instance of the DevAVDbDesignTimeUnitOfWork class.
        /// </summary>
        public DevAVDbDesignTimeUnitOfWork() {
        }

        IRepository<TaskAttachedFile, long> IDevAVDbUnitOfWork.AttachedFiles {
            get { return GetRepository((TaskAttachedFile x) => x.Id); }
        }

        IRepository<EmployeeTask, long> IDevAVDbUnitOfWork.Tasks {
            get { return GetRepository((EmployeeTask x) => x.Id); }
        }

        IRepository<Employee, long> IDevAVDbUnitOfWork.Employees {
            get { return GetRepository((Employee x) => x.Id); }
        }

        IRepository<CustomerCommunication, long> IDevAVDbUnitOfWork.Communications {
            get { return GetRepository((CustomerCommunication x) => x.Id); }
        }

        IRepository<CustomerEmployee, long> IDevAVDbUnitOfWork.CustomerEmployees {
            get { return GetRepository((CustomerEmployee x) => x.Id); }
        }

        IRepository<Customer, long> IDevAVDbUnitOfWork.Customers {
            get { return GetRepository((Customer x) => x.Id); }
        }

        IRepository<CustomerStore, long> IDevAVDbUnitOfWork.CustomerStores {
            get { return GetRepository((CustomerStore x) => x.Id); }
        }

        IRepository<Crest, long> IDevAVDbUnitOfWork.Crests {
            get { return GetRepository((Crest x) => x.Id); }
        }

        IRepository<Order, long> IDevAVDbUnitOfWork.Orders {
            get { return GetRepository((Order x) => x.Id); }
        }

        IRepository<OrderItem, long> IDevAVDbUnitOfWork.OrderItems {
            get { return GetRepository((OrderItem x) => x.Id); }
        }

        IRepository<Product, long> IDevAVDbUnitOfWork.Products {
            get { return GetRepository((Product x) => x.Id); }
        }

        IRepository<ProductCatalog, long> IDevAVDbUnitOfWork.ProductCatalogs {
            get { return GetRepository((ProductCatalog x) => x.Id); }
        }

        IRepository<ProductImage, long> IDevAVDbUnitOfWork.ProductImages {
            get { return GetRepository((ProductImage x) => x.Id); }
        }

        IRepository<Picture, long> IDevAVDbUnitOfWork.Pictures {
            get { return GetRepository((Picture x) => x.Id); }
        }

        IRepository<QuoteItem, long> IDevAVDbUnitOfWork.QuoteItems {
            get { return GetRepository((QuoteItem x) => x.Id); }
        }

        IRepository<Quote, long> IDevAVDbUnitOfWork.Quotes {
            get { return GetRepository((Quote x) => x.Id); }
        }

        IRepository<Evaluation, long> IDevAVDbUnitOfWork.Evaluations {
            get { return GetRepository((Evaluation x) => x.Id); }
        }

        IRepository<Probation, long> IDevAVDbUnitOfWork.Probations {
            get { return GetRepository((Probation x) => x.Id); }
        }

        IRepository<State, StateEnum> IDevAVDbUnitOfWork.States {
            get { return GetRepository((State x) => x.ShortName); }
        }

        IRepository<DatabaseVersion, long> IDevAVDbUnitOfWork.Version {
            get { return GetRepository((DatabaseVersion x) => x.Id); }
        }
    }
}
