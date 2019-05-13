using System;
using System.Windows;
using System.Windows.Controls;

namespace DevExpress.DevAV.Views {
    public partial class DevAVDbView : UserControl {
        public DevAVDbView() {
            InitializeComponent();
        }
        void OnNavButtonCloseClick(object sender, EventArgs e) {
            Application.Current.MainWindow.Close();
        }
    }
}
