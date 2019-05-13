using System;
using System.Windows;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;

namespace DevExpress.DevAV {
    public partial class MainWindow : ThemedWindow {
        public MainWindow() {
            InitializeComponent();
            if(Height > SystemParameters.VirtualScreenHeight || Width > SystemParameters.VirtualScreenWidth)
                WindowState = WindowState.Maximized;
            DevExpress.Utils.About.UAlgo.Default.DoEventObject(DevExpress.Utils.About.UAlgo.kDemo, DevExpress.Utils.About.UAlgo.pWPF, this); //DEMO_REMOVE
            EventManager.RegisterClassHandler(typeof(BarItem), BarItem.ItemClickEvent, new ItemClickEventHandler(OnBarItemClick), true);
            Xpf.Core.ThemeManager.AddThemeChangingHandler(this, (s, e) => {
                Logger.Log(string.Format("OutlookInspiredApp: Change Theme: {0}", Xpf.Core.ApplicationThemeHelper.ApplicationThemeName));
            });
        }

        void MainWindowLoaded(object sender, RoutedEventArgs e) {
            if(Left < 0 || Top < 0)
                WindowState = WindowState.Maximized;
        }
        void OnBarItemClick(object sender, ItemClickEventArgs e) {
            var barItem = (BarItem)sender;
            var content = barItem.Content ?? barItem.ActualCustomizationContent;
            if(content != null)
                Logger.Log(string.Format("OutlookInspiredApp: BarItemClick: {0}", content.ToString()));
        }
    }
}
