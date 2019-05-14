using System;
using System.Windows.Controls;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpf.Editors.Filtering;
using DevExpress.Xpf.Editors.Settings;

namespace DevExpress.DevAV.Views {
    public partial class CustomFilterView : UserControl {
        CustomFilterViewModel ViewModel { get { return (CustomFilterViewModel)DataContext; } }
        public CustomFilterView() {
            InitializeComponent();
        }
    }
    public class CustomFilterControlColumnsBehavior : FilterControlColumnsBehavior {
        protected override FilterColumn CreateFilterColumn(string columnCaption, BaseEditSettings editSettings, Type columnType, string fieldName) {
            return new CustomFilterColumn
            {
                ColumnCaption = columnCaption,
                EditSettings = editSettings,
                ColumnType = columnType,
                FieldName = fieldName
            };
        }
    }
    public class CustomFilterColumn : FilterColumn {
        public override bool IsValidClause(ClauseType clause) {
            return base.IsValidClause(clause) && !Equals(clause, ClauseType.Like) && !Equals(clause, ClauseType.NotLike);
        }
    }
}