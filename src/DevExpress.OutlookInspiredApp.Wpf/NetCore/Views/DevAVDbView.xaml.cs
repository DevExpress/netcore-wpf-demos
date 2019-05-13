using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Ribbon;
using DevExpress.Xpf.Accordion;
using System.Collections;
using DevExpress.DevAV.ViewModels;

namespace DevExpress.DevAV.Views {
    public partial class DevAVDbView : UserControl {
        public DevAVDbView() {
            InitializeComponent();
        }
    }
    public class OutlookChildrenSelector : IChildrenSelector {
        IEnumerable IChildrenSelector.SelectChildren(object item) {
            if(item is DevAVDbModuleDescription)
                return ((DevAVDbModuleDescription)item).FilterTreeViewModel.Categories;
            else if(item is FilterCategory) {
                return ((FilterCategory)item).FilterItems;
            }
            return null;
        }
    }
    public class ObjectsEqualityConverter : System.Windows.Data.IValueConverter {
        public bool Inverse { get; set; }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if(value == null)
                return value;
            var result = string.Equals(value.ToString(), parameter);
            return Inverse ? !result : result;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return (bool)value ? parameter : null;
        }
    }
    public class RibbonStyleSelectorItem : BarSplitButtonItem {
        static RibbonStyle globalRibbonStyle = DevExpress.Xpf.Ribbon.RibbonStyle.Office2010;
        public static RibbonStyle GlobalRibbonStyle {
            get { return globalRibbonStyle; }
            set {
                if(object.Equals(value, globalRibbonStyle))
                    return;
                globalRibbonStyle = value;
                GlobalRibbonStyleChanged(null, EventArgs.Empty);
            }
        }
        public static event EventHandler<EventArgs> GlobalRibbonStyleChanged;
        static RibbonStyleSelectorItem() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonStyleSelectorItem), new FrameworkPropertyMetadata(typeof(RibbonStyleSelectorItem)));
            GlobalRibbonStyleChanged = (s, e) => { };
        }
        ContentControl popupContentControl = new ContentControl();
        public RibbonStyleSelectorItem() {
            ActAsDropDown = true;
            ItemClickBehaviour = PopupItemClickBehaviour.CloseAllPopups;
            PopupControl = new PopupControlContainer() { Content = popupContentControl };
        }
        protected override void OnLoaded(object sender, RoutedEventArgs e) {
            base.OnLoaded(sender, e);
            SelectedRibbonStyle = GlobalRibbonStyle;
            GlobalRibbonStyleChanged += RibbonStyleSelectorItem_GlobalRibbonStyleChanged;
        }
        protected override void OnUnloaded(object sender, RoutedEventArgs e) {
            base.OnUnloaded(sender, e);
            GlobalRibbonStyleChanged -= RibbonStyleSelectorItem_GlobalRibbonStyleChanged;
        }
        void RibbonStyleSelectorItem_GlobalRibbonStyleChanged(object sender, EventArgs e) {
            SelectedRibbonStyle = GlobalRibbonStyle;
        }
        public RibbonStyle SelectedRibbonStyle {
            get { return (RibbonStyle)GetValue(SelectedRibbonStyleProperty); }
            set { SetValue(SelectedRibbonStyleProperty, value); }
        }
        public static readonly DependencyProperty SelectedRibbonStyleProperty =
            DependencyProperty.Register("SelectedRibbonStyle", typeof(RibbonStyle), typeof(RibbonStyleSelectorItem), new PropertyMetadata(DevExpress.Xpf.Ribbon.RibbonStyle.Office2010, OnSelectedRibbonStyleChanged));
        static void OnSelectedRibbonStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            GlobalRibbonStyle = (RibbonStyle)e.NewValue;
        }
        public DataTemplate PopupTemplate {
            get { return (DataTemplate)GetValue(PopupTemplateProperty); }
            set { SetValue(PopupTemplateProperty, value); }
        }
        public static readonly DependencyProperty PopupTemplateProperty =
            DependencyProperty.Register("PopupTemplate", typeof(DataTemplate), typeof(RibbonStyleSelectorItem), new PropertyMetadata(null, OnPopupTemplatePropertyChanged));
        static void OnPopupTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((RibbonStyleSelectorItem)d).popupContentControl.ContentTemplate = (DataTemplate)e.NewValue;
            ((RibbonStyleSelectorItem)d).popupContentControl.Content = new { Selector = d };
        }
    }
}
