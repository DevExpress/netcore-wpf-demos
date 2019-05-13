using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq.Expressions;

namespace DevExpress.DevAV.ViewModels {
    public class FilterTreeModelPageSpecificSettings<TSettings> : IFilterTreeModelPageSpecificSettings where TSettings : ApplicationSettingsBase {
        readonly string staticFiltersTitle;
        readonly string customFiltersTitle;
        readonly TSettings settings;
        readonly PropertyDescriptor customFiltersProperty;
        readonly PropertyDescriptor staticFiltersProperty;
        readonly IEnumerable<string> hiddenFilterProperties;
        readonly IEnumerable<string> additionalFilterProperties;

        public FilterTreeModelPageSpecificSettings(TSettings settings, string staticFiltersTitle,
            Expression<Func<TSettings, FilterInfoList>> getStaticFiltersExpression, Expression<Func<TSettings, FilterInfoList>> getCustomFiltersExpression,
            IEnumerable<string> hiddenFilterProperties = null, IEnumerable<string> additionalFilterProperties = null, string customFiltersTitle = "Custom Filters") {
            this.settings = settings;
            this.staticFiltersTitle = staticFiltersTitle;
            this.customFiltersTitle = customFiltersTitle;
            staticFiltersProperty = GetProperty(getStaticFiltersExpression);
            customFiltersProperty = GetProperty(getCustomFiltersExpression);
            this.hiddenFilterProperties = hiddenFilterProperties;
            this.additionalFilterProperties = additionalFilterProperties;
        }
        FilterInfoList IFilterTreeModelPageSpecificSettings.CustomFilters {
            get { return GetFilters(customFiltersProperty); }
            set { SetFilters(customFiltersProperty, value); }
        }
        FilterInfoList IFilterTreeModelPageSpecificSettings.StaticFilters {
            get { return GetFilters(staticFiltersProperty); }
            set { SetFilters(staticFiltersProperty, value); }
        }
        string IFilterTreeModelPageSpecificSettings.StaticFiltersTitle { get { return staticFiltersTitle; } }
        string IFilterTreeModelPageSpecificSettings.CustomFiltersTitle { get { return customFiltersTitle; } }
        IEnumerable<string> IFilterTreeModelPageSpecificSettings.HiddenFilterProperties { get { return hiddenFilterProperties; } }
        IEnumerable<string> IFilterTreeModelPageSpecificSettings.AdditionalFilterProperties { get { return additionalFilterProperties; } }
        void IFilterTreeModelPageSpecificSettings.SaveSettings() {
            settings.Save();
        }

        PropertyDescriptor GetProperty(Expression<Func<TSettings, FilterInfoList>> expression) {
            if(expression != null)
                return TypeDescriptor.GetProperties(settings)[GetPropertyName(expression)];
            return null;
        }
        FilterInfoList GetFilters(PropertyDescriptor property) {
            return property != null ? (FilterInfoList)property.GetValue(settings) : null;
        }
        void SetFilters(PropertyDescriptor property, FilterInfoList value) {
            if(property != null)
                property.SetValue(settings, value);
        }
        static string GetPropertyName(Expression<Func<TSettings, FilterInfoList>> expression) {
            MemberExpression memberExpression = expression.Body as MemberExpression;
            if(memberExpression == null) {
                throw new ArgumentException("expression");
            }
            return memberExpression.Member.Name;
        }
    }
}