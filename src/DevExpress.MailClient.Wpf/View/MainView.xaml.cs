using System.Windows.Controls;
using DevExpress.MailClient.ViewModel;

namespace DevExpress.MailClient.View {
    public partial class MainView : UserControl {
        public MainView() {
            InitializeComponent();
            DataContext = MainViewModel.Create();
        }
    }
}