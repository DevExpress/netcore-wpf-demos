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
    /// Represents the single Employee object view model.
    /// </summary>
    public partial class EmployeeViewModel : SingleObjectViewModel<Employee, long, IDevAVDbUnitOfWork> {

        /// <summary>
        /// Creates a new instance of EmployeeViewModel as a POCO view model.
        /// </summary>
        /// <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
        public static EmployeeViewModel Create(IUnitOfWorkFactory<IDevAVDbUnitOfWork> unitOfWorkFactory = null) {
            return ViewModelSource.Create(() => new EmployeeViewModel(unitOfWorkFactory));
        }

        /// <summary>
        /// Initializes a new instance of the EmployeeViewModel class.
        /// This constructor is declared protected to avoid undesired instantiation of the EmployeeViewModel type without the POCO proxy factory.
        /// </summary>
        /// <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
        protected EmployeeViewModel(IUnitOfWorkFactory<IDevAVDbUnitOfWork> unitOfWorkFactory = null)
            : base(unitOfWorkFactory ?? UnitOfWorkSource.GetUnitOfWorkFactory(), x => x.Employees, x => x.FullName) {
                }


        protected override void RefreshLookUpCollections(bool raisePropertyChanged) {
            base.RefreshLookUpCollections(raisePropertyChanged);
                AssignedEmployeeTasksDetailEntities = CreateAddRemoveDetailEntitiesViewModel(x => x.Tasks, x => x.AssignedEmployeeTasks);
        }
        /// <summary>
        /// The view model that contains a look-up collection of Tasks for the corresponding navigation property in the view.
        /// </summary>
        public IEntitiesViewModel<EmployeeTask> LookUpTasks {
            get {
                return GetLookUpEntitiesViewModel<EmployeeViewModel, EmployeeTask, long>(
                    propertyExpression: (EmployeeViewModel x) => x.LookUpTasks,
                    getRepositoryFunc: x => x.Tasks);
            }
        }
        /// <summary>
        /// The view model that contains a look-up collection of Pictures for the corresponding navigation property in the view.
        /// </summary>
        public IEntitiesViewModel<Picture> LookUpPictures {
            get {
                return GetLookUpEntitiesViewModel<EmployeeViewModel, Picture, long>(
                    propertyExpression: (EmployeeViewModel x) => x.LookUpPictures,
                    getRepositoryFunc: x => x.Pictures);
            }
        }
        /// <summary>
        /// The view model that contains a look-up collection of Probations for the corresponding navigation property in the view.
        /// </summary>
        public IEntitiesViewModel<Probation> LookUpProbations {
            get {
                return GetLookUpEntitiesViewModel<EmployeeViewModel, Probation, long>(
                    propertyExpression: (EmployeeViewModel x) => x.LookUpProbations,
                    getRepositoryFunc: x => x.Probations);
            }
        }
        /// <summary>
        /// The view model that contains a look-up collection of Communications for the corresponding navigation property in the view.
        /// </summary>
        public IEntitiesViewModel<CustomerCommunication> LookUpCommunications {
            get {
                return GetLookUpEntitiesViewModel<EmployeeViewModel, CustomerCommunication, long>(
                    propertyExpression: (EmployeeViewModel x) => x.LookUpCommunications,
                    getRepositoryFunc: x => x.Communications);
            }
        }
        /// <summary>
        /// The view model that contains a look-up collection of Orders for the corresponding navigation property in the view.
        /// </summary>
        public IEntitiesViewModel<Order> LookUpOrders {
            get {
                return GetLookUpEntitiesViewModel<EmployeeViewModel, Order, long>(
                    propertyExpression: (EmployeeViewModel x) => x.LookUpOrders,
                    getRepositoryFunc: x => x.Orders);
            }
        }
        /// <summary>
        /// The view model that contains a look-up collection of Products for the corresponding navigation property in the view.
        /// </summary>
        public IEntitiesViewModel<Product> LookUpProducts {
            get {
                return GetLookUpEntitiesViewModel<EmployeeViewModel, Product, long>(
                    propertyExpression: (EmployeeViewModel x) => x.LookUpProducts,
                    getRepositoryFunc: x => x.Products);
            }
        }
        /// <summary>
        /// The view model that contains a look-up collection of Quotes for the corresponding navigation property in the view.
        /// </summary>
        public IEntitiesViewModel<Quote> LookUpQuotes {
            get {
                return GetLookUpEntitiesViewModel<EmployeeViewModel, Quote, long>(
                    propertyExpression: (EmployeeViewModel x) => x.LookUpQuotes,
                    getRepositoryFunc: x => x.Quotes);
            }
        }
        /// <summary>
        /// The view model that contains a look-up collection of Evaluations for the corresponding navigation property in the view.
        /// </summary>
        public IEntitiesViewModel<Evaluation> LookUpEvaluations {
            get {
                return GetLookUpEntitiesViewModel<EmployeeViewModel, Evaluation, long>(
                    propertyExpression: (EmployeeViewModel x) => x.LookUpEvaluations,
                    getRepositoryFunc: x => x.Evaluations);
            }
        }

        public virtual AddRemoveDetailEntitiesViewModel<Employee, Int64, EmployeeTask, Int64, IDevAVDbUnitOfWork> AssignedEmployeeTasksDetailEntities { get; protected set; }

        /// <summary>
        /// The view model for the EmployeeOwnedTasks detail collection.
        /// </summary>
        public CollectionViewModelBase<EmployeeTask, EmployeeTask, long, IDevAVDbUnitOfWork> EmployeeOwnedTasksDetails {
            get {
                return GetDetailsCollectionViewModel<EmployeeViewModel, EmployeeTask, long, long?>(
                    propertyExpression: (EmployeeViewModel x) => x.EmployeeOwnedTasksDetails,
                    getRepositoryFunc: x => x.Tasks,
                    foreignKeyExpression: x => x.OwnerId,
                    navigationExpression: x => x.Owner);
            }
        }

        /// <summary>
        /// The view model for the EmployeeEmployees detail collection.
        /// </summary>
        public CollectionViewModelBase<CustomerCommunication, CustomerCommunication, long, IDevAVDbUnitOfWork> EmployeeEmployeesDetails {
            get {
                return GetDetailsCollectionViewModel<EmployeeViewModel, CustomerCommunication, long, long?>(
                    propertyExpression: (EmployeeViewModel x) => x.EmployeeEmployeesDetails,
                    getRepositoryFunc: x => x.Communications,
                    foreignKeyExpression: x => x.EmployeeId,
                    navigationExpression: x => x.Employee);
            }
        }

        /// <summary>
        /// The view model for the EmployeeOrders detail collection.
        /// </summary>
        public CollectionViewModelBase<Order, Order, long, IDevAVDbUnitOfWork> EmployeeOrdersDetails {
            get {
                return GetDetailsCollectionViewModel<EmployeeViewModel, Order, long, long?>(
                    propertyExpression: (EmployeeViewModel x) => x.EmployeeOrdersDetails,
                    getRepositoryFunc: x => x.Orders,
                    foreignKeyExpression: x => x.EmployeeId,
                    navigationExpression: x => x.Employee);
            }
        }

        /// <summary>
        /// The view model for the EmployeeProducts detail collection.
        /// </summary>
        public CollectionViewModelBase<Product, Product, long, IDevAVDbUnitOfWork> EmployeeProductsDetails {
            get {
                return GetDetailsCollectionViewModel<EmployeeViewModel, Product, long, long?>(
                    propertyExpression: (EmployeeViewModel x) => x.EmployeeProductsDetails,
                    getRepositoryFunc: x => x.Products,
                    foreignKeyExpression: x => x.EngineerId,
                    navigationExpression: x => x.Engineer);
            }
        }

        /// <summary>
        /// The view model for the EmployeeSupportedProducts detail collection.
        /// </summary>
        public CollectionViewModelBase<Product, Product, long, IDevAVDbUnitOfWork> EmployeeSupportedProductsDetails {
            get {
                return GetDetailsCollectionViewModel<EmployeeViewModel, Product, long, long?>(
                    propertyExpression: (EmployeeViewModel x) => x.EmployeeSupportedProductsDetails,
                    getRepositoryFunc: x => x.Products,
                    foreignKeyExpression: x => x.SupportId,
                    navigationExpression: x => x.Support);
            }
        }

        /// <summary>
        /// The view model for the EmployeeQuotes detail collection.
        /// </summary>
        public CollectionViewModelBase<Quote, Quote, long, IDevAVDbUnitOfWork> EmployeeQuotesDetails {
            get {
                return GetDetailsCollectionViewModel<EmployeeViewModel, Quote, long, long?>(
                    propertyExpression: (EmployeeViewModel x) => x.EmployeeQuotesDetails,
                    getRepositoryFunc: x => x.Quotes,
                    foreignKeyExpression: x => x.EmployeeId,
                    navigationExpression: x => x.Employee);
            }
        }

        /// <summary>
        /// The view model for the EmployeeEvaluationsCreatedBy detail collection.
        /// </summary>
        public CollectionViewModelBase<Evaluation, Evaluation, long, IDevAVDbUnitOfWork> EmployeeEvaluationsCreatedByDetails {
            get {
                return GetDetailsCollectionViewModel<EmployeeViewModel, Evaluation, long, long?>(
                    propertyExpression: (EmployeeViewModel x) => x.EmployeeEvaluationsCreatedByDetails,
                    getRepositoryFunc: x => x.Evaluations,
                    foreignKeyExpression: x => x.CreatedById,
                    navigationExpression: x => x.CreatedBy);
            }
        }

        /// <summary>
        /// The view model for the EmployeeEvaluations detail collection.
        /// </summary>
        public CollectionViewModelBase<Evaluation, Evaluation, long, IDevAVDbUnitOfWork> EmployeeEvaluationsDetails {
            get {
                return GetDetailsCollectionViewModel<EmployeeViewModel, Evaluation, long, long?>(
                    propertyExpression: (EmployeeViewModel x) => x.EmployeeEvaluationsDetails,
                    getRepositoryFunc: x => x.Evaluations,
                    foreignKeyExpression: x => x.EmployeeId,
                    navigationExpression: x => x.Employee);
            }
        }
    }
}
