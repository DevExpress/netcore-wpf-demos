using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.DevAV.DevAVDbDataModel;
using DevExpress.Mvvm;

namespace DevExpress.DevAV.ViewModels {
    partial class ProductCollectionViewModel : ISupportFiltering<Product>, IFilterTreeViewModelContainer<Product, long> {
        public virtual FilterTreeViewModel<Product, long> FilterTreeViewModel { get; set; }

        public void CreateCustomFilter() {
            Messenger.Default.Send(new CreateCustomFilterMessage<Product>());
        }
        protected override void OnEntitiesLoaded(IDevAVDbUnitOfWork unitOfWork, IEnumerable<ProductInfoWithSales> entities) {
            base.OnEntitiesLoaded(unitOfWork, entities);
            QueriesHelper.UpdateMonthlySales(unitOfWork.OrderItems, entities);
        }
        protected override void OnEntitiesAssigned(Func<ProductInfoWithSales> getSelectedEntityCallback) {
            base.OnEntitiesAssigned(getSelectedEntityCallback);
            if(SelectedEntity == null)
                SelectedEntity = Entities.FirstOrDefault();
        }
        #region ISupportFiltering
        Expression<Func<Product, bool>> ISupportFiltering<Product>.FilterExpression {
            get { return FilterExpression; }
            set { FilterExpression = value; }
        }
        #endregion
    }
}