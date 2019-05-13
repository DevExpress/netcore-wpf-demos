using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using DevExpress.Internal;
using DevExpress.Xpf.Core;

namespace DevExpress.RealtorWorld.Xpf {
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            ExceptionHelper.Initialize();
#if !DXCORE3
            DataDirectoryHelper.SetWebBrowserMode();
            ApplicationThemeHelper.ApplicationThemeName = Theme.MetropolisDark.Name;
#endif
            LoadPlugins();           
            base.OnStartup(e);
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata(200));
            SetCultureInfo();
        }
        static void SetCultureInfo() {
            CultureInfo demoCI = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            demoCI.NumberFormat.CurrencySymbol = "$";
            demoCI.DateTimeFormat = new DateTimeFormatInfo();
            Thread.CurrentThread.CurrentCulture = demoCI;
            CultureInfo demoUI = (CultureInfo)Thread.CurrentThread.CurrentUICulture.Clone();
            demoUI.NumberFormat.CurrencySymbol = "$";
            demoUI.DateTimeFormat = new DateTimeFormatInfo();
            Thread.CurrentThread.CurrentUICulture = demoUI;
        }
        #region LoadPlugins
        static void LoadPlugins() {
            foreach(string file in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "DevExpress.RealtorWorld.Xpf.Plugins.*.exe")) {
                Assembly plugin = Assembly.LoadFrom(file);
                if(plugin == null) continue;
                Type entry = plugin.GetType("Global.Program");
                if(entry == null) continue;
                MethodInfo start = entry.GetMethod("Start", BindingFlags.Static | BindingFlags.Public, null, new Type[] { }, null);
                if(start == null) continue;
                start.Invoke(null, new object[] { });
            }
        }
        #endregion
    }
}
#if CLICKONCE
namespace DevExpress.Internal.DemoLauncher {
    public static class ClickOnceEntryPoint {
        public static Tuple<Action, System.Threading.Tasks.Task<Window>> Run() {
            var done = new System.Threading.Tasks.TaskCompletionSource<Window>();
            Action run = () => {
                var app = new DevExpress.RealtorWorld.Xpf.App();
                app.InitializeComponent();
                typeof(Application).GetField("_startupUri", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(app, null);
                app.Startup += (s, e) => {
                    var window = new DevExpress.RealtorWorld.Xpf.View.MainWindow();
                    window.Show();
                    app.MainWindow = window;
                    done.SetResult(window);
                };
                app.Run();
            };
            return Tuple.Create(run, done.Task);
        }
    }
}
#endif
