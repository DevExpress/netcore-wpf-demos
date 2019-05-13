using DevExpress.Xpf.Grid;
using System.Windows.Controls;

namespace DevExpress.MailClient.View {
    public partial class NavigationMailView : UserControl {
        public NavigationMailView() {
            InitializeComponent();
            TreeListControl.AllowInfiniteGridSize = true;
        }
    }
}
