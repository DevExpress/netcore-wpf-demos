using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.DevAV.DevAVDbDataModel;
using DevExpress.Mvvm.POCO;

namespace DevExpress.DevAV.ViewModels {
    public partial class OrderViewModel {
        public virtual Tuple<IDevAVDbUnitOfWork, Order> SpreadsheetDataSource { get; set; }

        LinksViewModel linksViewModel;
        protected override void OnEntityChanged() {
            base.OnEntityChanged();
            if(Entity != null) { 
                SpreadsheetDataSource = new Tuple<IDevAVDbUnitOfWork, Order>(this.UnitOfWork, Entity);
                Logger.Log(string.Format("OutlookInspiredApp: Edit Order: {0}",
                    string.IsNullOrEmpty(Entity.InvoiceNumber) ? "<New>" : Entity.InvoiceNumber));
            }
        }
        public virtual void ResetAll() {
            Reset();
            ((OrderCollectionViewModel)this.ParentViewModel).Refresh();
        }
        public bool CanResetAll() {
            return CanReset();
        }
        public override void Save() {
            UnitOfWork.SaveChanges();
        }
        protected override bool HasValidationErrors() {
            return base.HasValidationErrors() || Entity == null || Entity.Customer == null;
        }
        public override bool CanSave() {
            return base.CanSave() && UnitOfWork.HasChanges() && Entity.OrderItems.Any();
        }
        public OrderCollectionViewModel OrderCollectionViewModel {
            get { return this.GetParentViewModel<OrderCollectionViewModel>(); }
        }
        public Order MasterEntity {
            get { return (OrderCollectionViewModel != null) ? OrderCollectionViewModel.SelectedEntity : null; }
        }
        public override bool CanDelete() {
            return MasterEntity != null;
        }
        public override void Delete() {
            OrderCollectionViewModel.Delete(MasterEntity);
        }
        public LinksViewModel LinksViewModel {
            get {
                if(linksViewModel == null)
                    linksViewModel = LinksViewModel.Create();
                return linksViewModel;
            }
        }
    }
}
