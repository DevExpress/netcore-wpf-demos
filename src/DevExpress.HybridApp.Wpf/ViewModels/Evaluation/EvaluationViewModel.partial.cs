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

namespace DevExpress.DevAV.ViewModels {
    partial class EvaluationViewModel {
        protected override Evaluation CreateEntity() {
            Evaluation entity = base.CreateEntity();
            entity.CreatedOn = DateTime.Now;
            return entity;
        }
    }
}
