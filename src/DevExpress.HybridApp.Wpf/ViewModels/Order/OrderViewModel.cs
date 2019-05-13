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
    /// Represents the single Order object view model.
    /// </summary>
    public partial class OrderViewModel : SingleObjectViewModel<Order, long, IDevAVDbUnitOfWork> {

        /// <summary>
        /// Creates a new instance of OrderViewModel as a POCO view model.
        /// </summary>
        /// <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
        public static OrderViewModel Create(IUnitOfWorkFactory<IDevAVDbUnitOfWork> unitOfWorkFactory = null) {
            return ViewModelSource.Create(() => new OrderViewModel(unitOfWorkFactory));
        }

        /// <summary>
        /// Initializes a new instance of the OrderViewModel class.
        /// This constructor is declared protected to avoid undesired instantiation of the OrderViewModel type without the POCO proxy factory.
        /// </summary>
        /// <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
        protected OrderViewModel(IUnitOfWorkFactory<IDevAVDbUnitOfWork> unitOfWorkFactory = null)
            : base(unitOfWorkFactory ?? UnitOfWorkSource.GetUnitOfWorkFactory(), x => x.Orders, x => x.InvoiceNumber) {
                }


        /// <summary>
        /// The view model that contains a look-up collection of Customers for the corresponding navigation property in the view.
        /// </summary>
        public IEntitiesViewModel<Customer> LookUpCustomers {
            get {
                return GetLookUpEntitiesViewModel<OrderViewModel, Customer, long>(
                    propertyExpression: (OrderViewModel x) => x.LookUpCustomers,
                    getRepositoryFunc: x => x.Customers);
            }
        }
        /// <summary>
        /// The view model that contains a look-up collection of Employees for the corresponding navigation property in the view.
        /// </summary>
        public IEntitiesViewModel<Employee> LookUpEmployees {
            get {
                return GetLookUpEntitiesViewModel<OrderViewModel, Employee, long>(
                    propertyExpression: (OrderViewModel x) => x.LookUpEmployees,
                    getRepositoryFunc: x => x.Employees);
            }
        }
        /// <summary>
        /// The view model that contains a look-up collection of CustomerStores for the corresponding navigation property in the view.
        /// </summary>
        public IEntitiesViewModel<CustomerStore> LookUpCustomerStores {
            get {
                return GetLookUpEntitiesViewModel<OrderViewModel, CustomerStore, long>(
                    propertyExpression: (OrderViewModel x) => x.LookUpCustomerStores,
                    getRepositoryFunc: x => x.CustomerStores);
            }
        }
        /// <summary>
        /// The view model that contains a look-up collection of OrderItems for the corresponding navigation property in the view.
        /// </summary>
        public IEntitiesViewModel<OrderItem> LookUpOrderItems {
            get {
                return GetLookUpEntitiesViewModel<OrderViewModel, OrderItem, long>(
                    propertyExpression: (OrderViewModel x) => x.LookUpOrderItems,
                    getRepositoryFunc: x => x.OrderItems);
            }
        }


        /// <summary>
        /// The view model for the OrderOrderItems detail collection.
        /// </summary>
        public CollectionViewModelBase<OrderItem, OrderItem, long, IDevAVDbUnitOfWork> OrderOrderItemsDetails {
            get {
                return GetDetailsCollectionViewModel<OrderViewModel, OrderItem, long, long?>(
                    propertyExpression: (OrderViewModel x) => x.OrderOrderItemsDetails,
                    getRepositoryFunc: x => x.OrderItems,
                    foreignKeyExpression: x => x.OrderId,
                    navigationExpression: x => x.Order);
            }
        }
    }
}
