using System.Collections.Generic;

namespace DevExpress.DevAV.ViewModels {
    public interface IFilterTreeModelPageSpecificSettings {
        string StaticFiltersTitle { get; }
        string CustomFiltersTitle { get; }
        FilterInfoList StaticFilters { get; set; }
        FilterInfoList CustomFilters { get; set; }
        IEnumerable<string> HiddenFilterProperties { get; }
        IEnumerable<string> AdditionalFilterProperties { get; }
        void SaveSettings();
    }
}