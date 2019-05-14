using System;
using DevExpress.Data.Filtering;
using DevExpress.Mvvm.POCO;
using System.Linq;

namespace DevExpress.DevAV.ViewModels {
    public class FilterItem {
        public static FilterItem Create(int entitiesCount, string name, CriteriaOperator filterCriteria, string imageUri, IFilterTreeViewModel filterTreeViewModel) {
            return ViewModelSource.Create(() => new FilterItem(entitiesCount, name, filterCriteria, imageUri, filterTreeViewModel));
        }

        protected FilterItem(int entitiesCount, string name, CriteriaOperator filterCriteria, string imageUri, IFilterTreeViewModel filterTreeViewModel) {
            this.Name = name;
            this.FilterCriteria = filterCriteria;
            this.ImageUri = imageUri;
            this.FilterTreeViewModel = filterTreeViewModel;
            Update(entitiesCount);
        }

        public virtual string Name { get; set; }

        public virtual CriteriaOperator FilterCriteria { get; set; }

        public virtual int EntitiesCount { get; protected set; }

        public virtual string DisplayText { get; protected set; }

        public virtual string ImageUri { get; protected set; }

        public IFilterTreeViewModel FilterTreeViewModel { get; protected set; }

        public bool IsCustomFilter {
            get {
                if(this.IsInDesignMode())
                    return false;
                var customCategory = FilterTreeViewModel.Categories.FirstOrDefault(x=> x.IsCustom);
                return customCategory == null ? false : customCategory.FilterItems.Contains(this);
            }
        }

        public void Update(int entitiesCount) {
            this.EntitiesCount = entitiesCount;
            DisplayText = string.Format("{0} ({1})", Name, entitiesCount);
        }

        public FilterItem Clone() {
            return FilterItem.Create(EntitiesCount, Name, FilterCriteria, ImageUri, FilterTreeViewModel);
        }
        public FilterItem Clone(string name, string imageUri) {
            return FilterItem.Create(EntitiesCount, name, FilterCriteria, imageUri, FilterTreeViewModel);
        }

        protected virtual void OnNameChanged() {
            Update(EntitiesCount);
        }
    }
}