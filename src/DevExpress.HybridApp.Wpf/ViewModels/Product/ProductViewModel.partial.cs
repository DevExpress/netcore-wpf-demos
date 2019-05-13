using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.DevAV.Common.Utils;
using DevExpress.DevAV.DevAVDbDataModel;
using DevExpress.DevAV;
using DevExpress.DevAV.Common.ViewModel;
using DevExpress.DevAV.ViewModels;
using System.IO;
using DevExpress.Mvvm.DataAnnotations;

namespace DevExpress.DevAV.ViewModels {
    partial class ProductViewModel {
        static double[] ZoomFactors = new double[] { 0.5, 0.6, 0.7, 0.8, 0.9, 1, 2, 3, 4, 5 };
        int zoomFactorIndex = 5;

        protected override void OnInitializeInRuntime() {
            base.OnInitializeInRuntime();
            ZoomFactor = 1;
        }
        public virtual Stream PdfDocument { get; set; }
        public virtual double ZoomFactor { get; set; }
        public virtual void ZoomIn() {
            if(zoomFactorIndex != ZoomFactors.Count() - 1)
                zoomFactorIndex++;
            ZoomFactor = ZoomFactors[zoomFactorIndex];
        }
        public virtual void ZoomOut() {
            if(zoomFactorIndex != 0)
                zoomFactorIndex--;
            ZoomFactor = ZoomFactors[zoomFactorIndex];
        }
        protected override Product CreateEntity() {
            Product entity = base.CreateEntity();
            entity.ProductionStart = DateTime.Now;
            entity.CurrentInventory = 1;
            return entity;
        }
        protected override void OnEntityChanged() {
            base.OnEntityChanged();
            PdfDocument = Entity != null && Entity.Catalog != null && Entity.Catalog.Count != 0 ? Entity.Catalog[0].PdfStream : null;
            Logger.Log("HybridApp: View Product");
        }
    }
}
