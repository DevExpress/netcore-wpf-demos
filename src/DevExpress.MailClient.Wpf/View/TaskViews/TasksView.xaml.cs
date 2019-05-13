using System.Windows.Controls;

namespace DevExpress.MailClient.View {
    public partial class TasksView : UserControl {
        public TasksView() {
            InitializeComponent();
        }
        void TableView_CellValueChanging(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e) {
            e.Source.PostEditor();
        }
    }
}
