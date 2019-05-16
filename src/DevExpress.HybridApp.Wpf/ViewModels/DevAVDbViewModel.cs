using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.DevAV.Common.Utils;
using DevExpress.DevAV.DevAVDbDataModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.ViewModel;

namespace DevExpress.DevAV.ViewModels {
    public partial class DevAVDbViewModel : DocumentsViewModel<DevAVDbModuleDescription, IDevAVDbUnitOfWork> {
        const string MyWorldGroup = "My World";
        const string OperationsGroup = "Operations";

        public DevAVDbViewModel()
            : base(UnitOfWorkSource.GetUnitOfWorkFactory()) {
            IsTablet = true;
        }

        protected override DevAVDbModuleDescription[] CreateModules() {
            var modules = new DevAVDbModuleDescription[] {
                new DevAVDbModuleDescription("Dashboard", "DashboardView", MyWorldGroup, FiltersSettings.GetDashboardFilterTree(this)),
                new DevAVDbModuleDescription("Tasks", "TaskCollectionView", MyWorldGroup, FiltersSettings.GetTasksFilterTree(this)),
                new DevAVDbModuleDescription("Employees", "EmployeeCollectionView", MyWorldGroup, FiltersSettings.GetEmployeesFilterTree(this)),
                new DevAVDbModuleDescription("Products", "ProductCollectionView", OperationsGroup, FiltersSettings.GetProductsFilterTree(this)),
                new DevAVDbModuleDescription("Customers", "CustomerCollectionView", OperationsGroup, FiltersSettings.GetCustomersFilterTree(this)),
                new DevAVDbModuleDescription("Sales", "OrderCollectionView", OperationsGroup, FiltersSettings.GetSalesFilterTree(this)),
                new DevAVDbModuleDescription("Opportunities", "QuoteCollectionView", OperationsGroup, FiltersSettings.GetOpportunitiesFilterTree(this))
            };
            foreach(var module in modules) {
                DevAVDbModuleDescription moduleRef = module;
                module.FilterTreeViewModel.NavigateAction = (() => {
                    Show(moduleRef);
                });
            }
            return modules;
        }

        protected override void OnActiveModuleChanged(DevAVDbModuleDescription oldModule) {
            base.OnActiveModuleChanged(oldModule);
            if(ActiveModule == null)
                return;
            if(ActiveModule.FilterTreeViewModel != null)
                ActiveModule.FilterTreeViewModel.SetViewModel(DocumentManagerService.ActiveDocument.Content);
            Logger.Log(string.Format("HybridApp: Select module: {0}", ActiveModule.ModuleTitle.ToUpper()));
        }

        protected override string GetModuleTitle(DevAVDbModuleDescription module) {
            return base.GetModuleTitle(module) + " - DevAV";
        }

        public override void OnLoaded(DevAVDbModuleDescription module) {
            base.OnLoaded(module);
#if !DXCORE3
            IsTablet = DeviceDetector.IsTablet;
#endif
            RegisterJumpList();
        }

        public override void SaveLogicalLayout() { }
        public override bool RestoreLogicalLayout() {
            return false;
        }

        LinksViewModel linksViewModel;
        public LinksViewModel LinksViewModel {
            get {
                if(linksViewModel == null)
                    linksViewModel = LinksViewModel.Create();
                return linksViewModel;
            }
        }
        public override DevAVDbModuleDescription DefaultModule {
            get {
                return Modules[2];
            }
        }
        public virtual bool IsTablet { get; set; }

        void RegisterJumpList() {
            
#if !CLICKONCE && !DXCORE3
            IApplicationJumpListService jumpListService = this.GetService<IApplicationJumpListService>();
            jumpListService.Items.AddOrReplace("Become a UI Superhero", "Explore DevExpress Universal", UniversalIcon, () => { LinksViewModel.UniversalSubscription(); });
            jumpListService.Items.AddOrReplace("Online Tutorials", "Explore DevExpress Universal", TutorialsIcon, () => { LinksViewModel.GettingStarted(); });
            jumpListService.Items.AddOrReplace("Buy Now", "Explore DevExpress Universal", BuyIcon, () => { LinksViewModel.BuyNow(); });
            jumpListService.Apply();
#endif
        }
        ImageSource UniversalIcon { get { return new BitmapImage(new Uri("pack://application:,,,/DevExpress.HybridApp.Wpf;component/Resources/Universal.ico")); } }
        ImageSource TutorialsIcon { get { return new BitmapImage(new Uri("pack://application:,,,/DevExpress.HybridApp.Wpf;component/Resources/WPF.ico")); } }
        ImageSource BuyIcon { get { return new BitmapImage(new Uri("pack://application:,,,/DevExpress.HybridApp.Wpf;component/Resources/DevExpress.ico")); } }
    }

    public partial class DevAVDbModuleDescription : ModuleDescription<DevAVDbModuleDescription> {
        public DevAVDbModuleDescription(string title, string documentType, string group, IFilterTreeViewModel filterTreeViewModel)
            : base(title, documentType, group, null) {
            ImageSource = new Uri(string.Format(@"pack://application:,,,/DevExpress.HybridApp.Wpf;component/Resources/Menu/{0}.png", title));
            FilterTreeViewModel = filterTreeViewModel;
        }
        public Uri ImageSource { get; private set; }

        public IFilterTreeViewModel FilterTreeViewModel { get; private set; }
    }

}