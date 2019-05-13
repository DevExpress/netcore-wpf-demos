using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.LayoutControl;
using System.Threading;
using System.Globalization;

namespace DevExpress.DevAV.Views {
    public partial class CustomerView : UserControl {
        public CustomerView() {
            var culture = new CultureInfo("en-us");
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
        void CityGroupSizeChanged(object sender, SizeChangedEventArgs e) {
            ((LayoutGroup)sender).ItemLabelsAlignment = (e.NewSize.Width < 360) ? LayoutItemLabelsAlignment.Local : LayoutItemLabelsAlignment.Default;
        }
    }
}
