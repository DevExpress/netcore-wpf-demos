using System.Windows;
using DevExpress.Xpf.Core;

namespace DevExpress.DevAV {
    public partial class SplashScreenWindow : Window, ISplashScreen {
        public SplashScreenWindow() {
            this.Visibility = System.Diagnostics.Debugger.IsAttached ? Visibility.Hidden : Visibility.Visible;
            InitializeComponent();
            this.CopyrightText.Text = AssemblyInfo.AssemblyCopyright;
        }

        void ISplashScreen.CloseSplashScreen() {
            this.Close();
        }
        void ISplashScreen.Progress(double value) {
            progressBar.Value = value;
        }
        void ISplashScreen.SetProgressState(bool isIndeterminate) {
        }
    }
}
