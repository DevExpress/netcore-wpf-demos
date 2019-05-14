using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using System.Runtime.InteropServices;
using DevExpress.Images;
using DevExpress.Internal;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Core;

namespace DevExpress.DevAV {
    public partial class App : Application {
        static IDisposable singleInstanceApplicationGuard;

        protected override void OnStartup(StartupEventArgs e) {
            ExceptionHelper.Initialize();
            AppDomain.CurrentDomain.AssemblyResolve += OnCurrentDomainAssemblyResolve;
            DevAVDataDirectoryHelper.LocalPrefix = "WpfOutlookInspiredApp";
#if !DXCORE3
            AssemblyResolver.Subcribe();
            ServiceContainer.Default.RegisterService(new ApplicationJumpListService());
            Theme.RegisterPredefinedPaletteThemes();
#endif
            ImagesAssemblyLoader.Load();
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata(200));
            LoadPlugins();
            base.OnStartup(e);
            ViewLocator.Default = new ViewLocator(typeof(App).Assembly);
            bool exit;
            singleInstanceApplicationGuard = DevAVDataDirectoryHelper.SingleInstanceApplicationGuard("DevExpressWpfOutlookInspiredApp", out exit);
            if(exit) {
                Shutdown();
                return;
            }
#if !DXCORE3
            Theme.TouchlineDark.ShowInThemeSelector = false;
            ApplicationThemeHelper.ApplicationThemeName = Theme.Office2019Colorful.Name;
#endif
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
        static Assembly OnCurrentDomainAssemblyResolve(object sender, ResolveEventArgs args) {
            string partialName = DevExpress.Utils.AssemblyHelper.GetPartialName(args.Name).ToLower();
            if(partialName == "entityframework" || partialName == "system.data.sqlite" || partialName == "system.data.sqlite.ef6") {
                string path = Path.Combine(Path.GetDirectoryName(typeof(App).Assembly.Location), partialName + ".dll");
                return Assembly.LoadFrom(path);
            }
            return null;
        }
#region LoadPlugins
        static void LoadPlugins() {
#if !DXCORE3
            foreach(string file in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "DevExpress.OutlookInspiredApp.Wpf.Plugins.*.exe")) {
                Assembly.LoadFrom(file)
                    .With(x => x.GetType("Global.Program"))
                    .With(x => x.GetMethod("Start", BindingFlags.Static | BindingFlags.Public, null, new Type[] { }, null))
                    .Do(x => x.Invoke(null, new object[] { }));
            }
#endif
        }
        #endregion
        public static string ApplicationID {
            get { return string.Format("Components_{0}_Demo_Center_OutlookInspired_{0}", AssemblyInfo.VersionShort.Replace(".", "_")); }
        }
    }
    [Guid("86882B9F-1EAE-41D9-B9CF-BD7ACCA51523"), ComVisible(true)]
    public class OutlookInspiredAppNotificationActivator : ToastNotificationActivator {
        public override void OnActivate(string arguments, System.Collections.Generic.Dictionary<string, string> data) {
        }
    }
}
#if CLICKONCE
namespace DevExpress.Internal.DemoLauncher {
    public static class ClickOnceEntryPoint {
        public static Tuple<Action, System.Threading.Tasks.Task<Window>> Run() {
            var done = new System.Threading.Tasks.TaskCompletionSource<Window>();
            Action run = () => {
                var app = new DevExpress.DevAV.App();
                app.InitializeComponent();
                typeof(Application).GetField("_startupUri", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(app, null);
                app.Startup += (s, e) => {
                    var window = new DevExpress.DevAV.MainWindow();
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