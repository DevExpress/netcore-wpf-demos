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
    /// Represents the Evaluations collection view model.
    /// </summary>
    public partial class EvaluationCollectionViewModel : CollectionViewModel<Evaluation, long, IDevAVDbUnitOfWork> {

        /// <summary>
        /// Creates a new instance of EvaluationCollectionViewModel as a POCO view model.
        /// </summary>
        /// <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
        public static EvaluationCollectionViewModel Create(IUnitOfWorkFactory<IDevAVDbUnitOfWork> unitOfWorkFactory = null, UnitOfWorkPolicy unitOfWorkPolicy = UnitOfWorkPolicy.Individual) {
            return ViewModelSource.Create(() => new EvaluationCollectionViewModel(unitOfWorkFactory, unitOfWorkPolicy));
        }

        /// <summary>
        /// Initializes a new instance of the EvaluationCollectionViewModel class.
        /// This constructor is declared protected to avoid undesired instantiation of the EvaluationCollectionViewModel type without the POCO proxy factory.
        /// </summary>
        /// <param name="unitOfWorkFactory">A factory used to create a unit of work instance.</param>
        protected EvaluationCollectionViewModel(IUnitOfWorkFactory<IDevAVDbUnitOfWork> unitOfWorkFactory = null, UnitOfWorkPolicy unitOfWorkPolicy = UnitOfWorkPolicy.Individual)
            : base(unitOfWorkFactory ?? UnitOfWorkSource.GetUnitOfWorkFactory(), x => x.Evaluations, unitOfWorkPolicy: unitOfWorkPolicy) {
        }
    }
}