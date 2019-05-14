using System;
using System.Windows;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.DevAV.Common.ViewModel;

namespace DevExpress.DevAV.Common.View {
    public class MainWindowService : ServiceBase, IMainWindowService {
        public string Title {
            get { return Application.Current.With(a => a.MainWindow).With(w => w.Title); }
            set { Application.Current.With(a => a.MainWindow).Do(w => w.Title = value); }
        }
    }
}
