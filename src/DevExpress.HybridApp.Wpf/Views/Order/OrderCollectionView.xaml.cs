using System.Globalization;
using System.Threading;
using System.Windows.Controls;

namespace DevExpress.DevAV.Views {
    public partial class OrderCollectionView : UserControl {
        public OrderCollectionView() {
            var culture = new CultureInfo("en-us");
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}
