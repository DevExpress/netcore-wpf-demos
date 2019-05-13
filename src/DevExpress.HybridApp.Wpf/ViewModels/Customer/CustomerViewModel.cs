using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.DataModel;
using DevExpress.Mvvm.ViewModel;
using DevExpress.DevAV.DevAVDbDataModel;
using DevExpress.DevAV.Common;
using DevExpress.DevAV;

namespace DevExpress.DevAV.ViewModels {

    /// <summary>
    /// Represents the single Customer object view model.
    /// </summary>
    public partial class CustomerViewModel : SingleObjectViewModel<Customer, long, IDevAVDbUnitOfWork> {

        /// <summary>
        /// Creates a new instance of CustomerViewModel as a POCO view model.
        /// </summary>
        /// <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
        public static CustomerViewModel Create(IUnitOfWorkFactory<IDevAVDbUnitOfWork> unitOfWorkFactory = null) {
            return ViewModelSource.Create(() => new CustomerViewModel(unitOfWorkFactory));
        }

        /// <summary>
        /// Initializes a new instance of the CustomerViewModel class.
        /// This constructor is declared protected to avoid undesired instantiation of the CustomerViewModel type without the POCO proxy factory.
        /// </summary>
        /// <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
        protected CustomerViewModel(IUnitOfWorkFactory<IDevAVDbUnitOfWork> unitOfWorkFactory = null)
            : base(unitOfWorkFactory ?? UnitOfWorkSource.GetUnitOfWorkFactory(), x => x.Customers, x => x.Name) {
                }


        /// <summary>
        /// The view model that contains a look-up collection of CustomerEmployees for the corresponding navigation property in the view.
        /// </summary>
        public IEntitiesViewModel<CustomerEmployee> LookUpCustomerEmployees {
            get {
                return GetLookUpEntitiesViewModel<CustomerViewModel, CustomerEmployee, long>(
                    propertyExpression: (CustomerViewModel x) => x.LookUpCustomerEmployees,
                    getRepositoryFunc: x => x.CustomerEmployees);
            }
        }
        /// <summary>
        /// The view model that contains a look-up collection of CustomerStores for the corresponding navigation property in the view.
        /// </summary>
        public IEntitiesViewModel<CustomerStore> LookUpCustomerStores {
            get {
                return GetLookUpEntitiesViewModel<CustomerViewModel, CustomerStore, long>(
                    propertyExpression: (CustomerViewModel x) => x.LookUpCustomerStores,
                    getRepositoryFunc: x => x.CustomerStores);
            }
        }
        /// <summary>
        /// The view model that contains a look-up collection of Orders for the corresponding navigation property in the view.
        /// </summary>
        public IEntitiesViewModel<Order> LookUpOrders {
            get {
                return GetLookUpEntitiesViewModel<CustomerViewModel, Order, long>(
                    propertyExpression: (CustomerViewModel x) => x.LookUpOrders,
                    getRepositoryFunc: x => x.Orders);
            }
        }
        /// <summary>
        /// The view model that contains a look-up collection of Quotes for the corresponding navigation property in the view.
        /// </summary>
        public IEntitiesViewModel<Quote> LookUpQuotes {
            get {
                return GetLookUpEntitiesViewModel<CustomerViewModel, Quote, long>(
                    propertyExpression: (CustomerViewModel x) => x.LookUpQuotes,
                    getRepositoryFunc: x => x.Quotes);
            }
        }


        /// <summary>
        /// The view model for the CustomerEmployees detail collection.
        /// </summary>
        public CollectionViewModelBase<CustomerEmployee, CustomerEmployee, long, IDevAVDbUnitOfWork> CustomerEmployeesDetails {
            get {
                return GetDetailsCollectionViewModel<CustomerViewModel, CustomerEmployee, long, long?>(
                    propertyExpression: (CustomerViewModel x) => x.CustomerEmployeesDetails,
                    getRepositoryFunc: x => x.CustomerEmployees,
                    foreignKeyExpression: x => x.CustomerId,
                    navigationExpression: x => x.Customer);
            }
        }

        /// <summary>
        /// The view model for the CustomerCustomerStores detail collection.
        /// </summary>
        public CollectionViewModelBase<CustomerStore, CustomerStore, long, IDevAVDbUnitOfWork> CustomerCustomerStoresDetails {
            get {
                return GetDetailsCollectionViewModel<CustomerViewModel, CustomerStore, long, long?>(
                    propertyExpression: (CustomerViewModel x) => x.CustomerCustomerStoresDetails,
                    getRepositoryFunc: x => x.CustomerStores,
                    foreignKeyExpression: x => x.CustomerId,
                    navigationExpression: x => x.Customer);
            }
        }

        /// <summary>
        /// The view model for the CustomerOrders detail collection.
        /// </summary>
        public CollectionViewModelBase<Order, Order, long, IDevAVDbUnitOfWork> CustomerOrdersDetails {
            get {
                return GetDetailsCollectionViewModel<CustomerViewModel, Order, long, long?>(
                    propertyExpression: (CustomerViewModel x) => x.CustomerOrdersDetails,
                    getRepositoryFunc: x => x.Orders,
                    foreignKeyExpression: x => x.CustomerId,
                    navigationExpression: x => x.Customer);
            }
        }

        /// <summary>
        /// The view model for the CustomerQuotes detail collection.
        /// </summary>
        public CollectionViewModelBase<Quote, Quote, long, IDevAVDbUnitOfWork> CustomerQuotesDetails {
            get {
                return GetDetailsCollectionViewModel<CustomerViewModel, Quote, long, long?>(
                    propertyExpression: (CustomerViewModel x) => x.CustomerQuotesDetails,
                    getRepositoryFunc: x => x.Quotes,
                    foreignKeyExpression: x => x.CustomerId,
                    navigationExpression: x => x.Customer);
            }
        }
    }
}
