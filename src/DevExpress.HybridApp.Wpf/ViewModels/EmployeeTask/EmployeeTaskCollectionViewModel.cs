using System;
using System.Linq;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.DataModel;
using DevExpress.Mvvm.ViewModel;
using DevExpress.DevAV.DevAVDbDataModel;
using DevExpress.DevAV.Common;
using DevExpress.DevAV;

namespace DevExpress.DevAV.ViewModels {

    /// <summary>
    /// Represents the Tasks collection view model.
    /// </summary>
    public partial class EmployeeTaskCollectionViewModel : CollectionViewModel<EmployeeTask, long, IDevAVDbUnitOfWork> {

        /// <summary>
        /// Creates a new instance of EmployeeTaskCollectionViewModel as a POCO view model.
        /// </summary>
        /// <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
        public static EmployeeTaskCollectionViewModel Create(IUnitOfWorkFactory<IDevAVDbUnitOfWork> unitOfWorkFactory = null, UnitOfWorkPolicy unitOfWorkPolicy = UnitOfWorkPolicy.Individual) {
            return ViewModelSource.Create(() => new EmployeeTaskCollectionViewModel(unitOfWorkFactory, unitOfWorkPolicy));
        }

        /// <summary>
        /// Initializes a new instance of the EmployeeTaskCollectionViewModel class.
        /// This constructor is declared protected to avoid undesired instantiation of the EmployeeTaskCollectionViewModel type without the POCO proxy factory.
        /// </summary>
        /// <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
        protected EmployeeTaskCollectionViewModel(IUnitOfWorkFactory<IDevAVDbUnitOfWork> unitOfWorkFactory = null, UnitOfWorkPolicy unitOfWorkPolicy = UnitOfWorkPolicy.Individual)
            : base(unitOfWorkFactory ?? UnitOfWorkSource.GetUnitOfWorkFactory(), x => x.Tasks, unitOfWorkPolicy: unitOfWorkPolicy) {
        }
    }
}