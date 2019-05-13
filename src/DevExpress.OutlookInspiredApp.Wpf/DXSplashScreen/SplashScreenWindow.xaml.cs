using System.Windows;
using DevExpress.Xpf.Core;

namespace DevExpress.DevAV {
    public partial class SplashScreenWindow : Window, ISplashScreen {
        public SplashScreenWindow() {
            InitializeComponent();
            this.Visibility = System.Diagnostics.Debugger.IsAttached ? Visibility.Hidden : Visibility.Visible;
            this.CopyrightText.Text = AssemblyInfo.AssemblyCopyright;
        }

        #region ISplashScreen
        void ISplashScreen.Progress(double value) {
            progressBar.Value = value;
        }
        void ISplashScreen.CloseSplashScreen() {
            this.Close();
        }
        void ISplashScreen.SetProgressState(bool isIndeterminate) {
            //progressBar.IsIndeterminate = isIndeterminate;
        }
        #endregion
    }
}
