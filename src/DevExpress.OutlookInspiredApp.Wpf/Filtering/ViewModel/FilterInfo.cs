using System.Collections.Generic;

namespace DevExpress.DevAV.ViewModels {
    public class FilterInfo {
        public string Name { get; set; }
        public string FilterCriteria { get; set; }
        public string ImageUri { get; set; }
    }

    public class FilterInfoList : List<FilterInfo> {
        public FilterInfoList() { }
        public FilterInfoList(IEnumerable<FilterInfo> filters)
            : base(filters) {
        }
    }
}