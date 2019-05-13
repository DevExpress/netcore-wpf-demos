using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.MailClient.ViewModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.ModuleInjection;
using DevExpress.Xpf.Accordion;

namespace DevExpress.MailClient.Utils {
    public class AccordionControlStrategy : CommonStrategyBase<AccordionControl, AccordionControlWrapper> {
        protected override void InitializeCore() {
            base.InitializeCore();
            Subscribe();
            InitItemTemplate();
        }
        protected override void UninitializeCore() {
            Unsubscribe();
            base.UninitializeCore();
        }
        void Subscribe() {
            ViewModels.CollectionChanged += OnCollectionChanged;
            Wrapper.Target.SelectedRootItemChanged += OnSelectedRootItemChanged;
            Wrapper.Target.Initialized += OnTargetInitialized;
        }
        void Unsubscribe() {
            Wrapper.Target.SelectedRootItemChanged -= OnSelectedRootItemChanged;
            ViewModels.CollectionChanged -= OnCollectionChanged;
            Wrapper.Target.Initialized -= OnTargetInitialized;
        }
        void OnSelectedRootItemChanged(object sender, EventArgs e) {
            SelectedViewModel = ((HeaderViewModel)Wrapper.SelectedItem).Content.First();
        }
        void OnTargetInitialized(object sender, EventArgs e) {
            ConfigAccordion();
        }
        void OnCollectionChanged(object sender, EventArgs e) {
            ConfigAccordion();
        }
        void ConfigAccordion() {
            if(!Wrapper.Target.IsInitialized)
                return;
            var source = GetWrapperItemsSource();
            Wrapper.ItemsSource = source;
            if(source != null && source.Any() && Wrapper.SelectedItem == null)
                Wrapper.SelectedItem = source.First();
        }
        ObservableCollection<HeaderViewModel> GetWrapperItemsSource() {
            if(ViewModels == null || !ViewModels.Any())
                return null;
            var collection = ViewModels.Select(x => ((ViewModel.NavigationViewModelBase)x).HeaderViewModel);
            return new ObservableCollection<HeaderViewModel>(collection);
        }
        protected override void OnSelectedViewModelChanged(object oldValue, object newValue) {
            base.OnSelectedViewModelChanged(oldValue, newValue);
            Wrapper.SelectedItem = ((ViewModel.NavigationViewModelBase)newValue).HeaderViewModel;
        }
        protected virtual void InitItemTemplate() {
            if(Wrapper.ItemTemplate == null && Wrapper.ItemTemplateSelector == null)
                Wrapper.ItemTemplateSelector = ViewSelector;
        }
        protected override void OnRemoved(object viewModel) {
            base.OnRemoved(viewModel);
            if(viewModel == SelectedViewModel)
                SelectedViewModel = null;
        }
        protected override void OnClear() {
            base.OnClear();
            SelectedViewModel = null;
        }
    }
    public class AccordionControlWrapper : DependencyObject, ITargetWrapper<AccordionControl> {
        public AccordionControl Target { get; set; }
        public object ItemsSource { get { return Target.ItemsSource; } set { Target.ItemsSource = (IEnumerable<object>)value; } }
        public virtual DataTemplate ItemTemplate { get { return Target.ItemTemplate; } set { Target.ItemTemplate = value; } }
        public virtual DataTemplateSelector ItemTemplateSelector { get { return Target.ItemTemplateSelector; } set { Target.ItemTemplateSelector = value; } }
        public object SelectedItem { get { return Target.SelectedRootItem; } set { Target.SelectedRootItem = value; } }
    }
}
