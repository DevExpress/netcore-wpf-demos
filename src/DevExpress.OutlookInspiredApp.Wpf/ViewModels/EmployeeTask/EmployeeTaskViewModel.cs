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
    /// Represents the single EmployeeTask object view model.
    /// </summary>
    public partial class EmployeeTaskViewModel : SingleObjectViewModel<EmployeeTask, long, IDevAVDbUnitOfWork> {

        /// <summary>
        /// Creates a new instance of EmployeeTaskViewModel as a POCO view model.
        /// </summary>
        /// <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
        public static EmployeeTaskViewModel Create(IUnitOfWorkFactory<IDevAVDbUnitOfWork> unitOfWorkFactory = null) {
            return ViewModelSource.Create(() => new EmployeeTaskViewModel(unitOfWorkFactory));
        }

        /// <summary>
        /// Initializes a new instance of the EmployeeTaskViewModel class.
        /// This constructor is declared protected to avoid undesired instantiation of the EmployeeTaskViewModel type without the POCO proxy factory.
        /// </summary>
        /// <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
        protected EmployeeTaskViewModel(IUnitOfWorkFactory<IDevAVDbUnitOfWork> unitOfWorkFactory = null)
            : base(unitOfWorkFactory ?? UnitOfWorkSource.GetUnitOfWorkFactory(), x => x.Tasks, x => x.Subject) {
                }


        protected override void RefreshLookUpCollections(bool raisePropertyChanged) {
            base.RefreshLookUpCollections(raisePropertyChanged);
                AssignedEmployeesDetailEntities = CreateAddRemoveDetailEntitiesViewModel(x => x.Employees, x => x.AssignedEmployees);
        }
        /// <summary>
        /// The view model that contains a look-up collection of AttachedFiles for the corresponding navigation property in the view.
        /// </summary>
        public IEntitiesViewModel<TaskAttachedFile> LookUpAttachedFiles {
            get {
                return GetLookUpEntitiesViewModel<EmployeeTaskViewModel, TaskAttachedFile, long>(
                    propertyExpression: (EmployeeTaskViewModel x) => x.LookUpAttachedFiles,
                    getRepositoryFunc: x => x.AttachedFiles);
            }
        }
        /// <summary>
        /// The view model that contains a look-up collection of Employees for the corresponding navigation property in the view.
        /// </summary>
        public IEntitiesViewModel<Employee> LookUpEmployees {
            get {
                return GetLookUpEntitiesViewModel<EmployeeTaskViewModel, Employee, long>(
                    propertyExpression: (EmployeeTaskViewModel x) => x.LookUpEmployees,
                    getRepositoryFunc: x => x.Employees);
            }
        }
        /// <summary>
        /// The view model that contains a look-up collection of CustomerEmployees for the corresponding navigation property in the view.
        /// </summary>
        public IEntitiesViewModel<CustomerEmployee> LookUpCustomerEmployees {
            get {
                return GetLookUpEntitiesViewModel<EmployeeTaskViewModel, CustomerEmployee, long>(
                    propertyExpression: (EmployeeTaskViewModel x) => x.LookUpCustomerEmployees,
                    getRepositoryFunc: x => x.CustomerEmployees);
            }
        }

    public virtual AddRemoveDetailEntitiesViewModel<EmployeeTask, Int64, Employee, Int64, IDevAVDbUnitOfWork> AssignedEmployeesDetailEntities { get; protected set; }

        /// <summary>
        /// The view model for the EmployeeTaskAttachedFiles detail collection.
        /// </summary>
        public CollectionViewModelBase<TaskAttachedFile, TaskAttachedFile, long, IDevAVDbUnitOfWork> EmployeeTaskAttachedFilesDetails {
            get {
                return GetDetailsCollectionViewModel<EmployeeTaskViewModel, TaskAttachedFile, long, long?>(
                    propertyExpression: (EmployeeTaskViewModel x) => x.EmployeeTaskAttachedFilesDetails,
                    getRepositoryFunc: x => x.AttachedFiles,
                    foreignKeyExpression: x => x.EmployeeTaskId,
                    navigationExpression: x => x.EmployeeTask);
            }
        }
    }
}
