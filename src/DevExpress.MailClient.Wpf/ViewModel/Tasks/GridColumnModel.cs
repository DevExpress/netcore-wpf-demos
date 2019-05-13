using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using System.Windows;
using DevExpress.Mvvm.POCO;
using System;

namespace DevExpress.MailClient.ViewModel {
    public class GridColumnViewModel {
        public static GridColumnViewModel Create() {
            return ViewModelSource.Create(() => new GridColumnViewModel());
        }
        public static GridColumnViewModel Create(Action<GridColumnViewModel> objectInitializer) {
            var result = Create();
            objectInitializer(result);
            return result;
        }

        #region Props
        public string FieldName { get; set; }
        public string DisplayName { get; set; }
        public HorizontalAlignment HorizontalHeaderContentAlignment { get; set; }
        public DefaultBoolean AllowFiltering { get; set; }
        public DefaultBoolean AllowSorting { get; set; }
        public DefaultBoolean AllowEditing { get; set; }
        public ColumnGroupInterval GroupInterval { get; set; }

        public virtual ColumnSortOrder SortOrder { get; set; }
        public virtual int Index { get; set; }
        public virtual bool IsGrouped { get; set; }
        public virtual double Width { get; set; }
        public virtual bool IsVisible { get; set; }
        #endregion

        protected GridColumnViewModel() {
            AllowFiltering = DefaultBoolean.True;
            AllowSorting = DefaultBoolean.True;
            Width = double.NaN;
            IsVisible = true;
        }
    }
}
