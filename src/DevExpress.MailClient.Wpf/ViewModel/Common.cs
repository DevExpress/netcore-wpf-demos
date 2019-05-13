using DevExpress.MailClient.DataProvider;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.Mvvm.ModuleInjection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.DevAV;

namespace DevExpress.MailClient.ViewModel {
    public enum ModuleType { Mail, Tasks, Calendar, Contacts }
    public static class Regions {
        public static string Navigation { get { return "Navigation"; } }
        public static string Documents { get { return "Documents"; } }
    }

    public static class Modules {
        public static string Mail { get { return "Mail"; } }
        public static string Tasks { get { return "Tasks"; } }
        public static string Calendar { get { return "Calendar"; } }
        public static string Contacts { get { return "Contacts"; } }
    }

    public class ContentViewModelInjectedMessage {
        public ModuleType ModuleType { get; private set; }
        public object ViewModel { get; private set; }

        public ContentViewModelInjectedMessage(ModuleType moduleType, object viewModel) {
            ModuleType = moduleType;
            ViewModel = viewModel;
        }
    }
    public abstract class NavigationViewModelBase {
        public Uri Icon { get; protected set; }
        public string Header { get; protected set; }
        public virtual object ContentViewModel { get; protected set; }
        protected ModuleType ModuleType { get; private set; }
        protected IModuleManager Manager { get { return ModuleManager.DefaultManager; } }

        public HeaderViewModel HeaderViewModel { get; set; }

        protected NavigationViewModelBase(ModuleType moduleType) {
            ModuleType = moduleType;
            Initialize();
            if(this.IsInDesignMode())
                InitializeInDesignMode();
            Messenger.Default.Register<ContentViewModelInjectedMessage>(this, OnViewModelInjectedMessage);
        }
        protected virtual void Initialize() { }
        protected virtual void InitializeInDesignMode() { }
        protected virtual void OnContentViewModelChanged(object oldValue) { }

        void OnViewModelInjectedMessage(ContentViewModelInjectedMessage message) {
            if(ModuleType == message.ModuleType)
                ContentViewModel = message.ViewModel;
        }
    }
    public class HeaderViewModel {
        public static HeaderViewModel Create() {
            return ViewModelSource.Create(() => new HeaderViewModel());
        }
        public ImageSource Icon { get; protected set; }
        public string Header { get; protected set; }
        public List<NavigationViewModelBase> Content { get; protected set; }
        public void Init(NavigationViewModelBase modelBase) {
            Content = new List<NavigationViewModelBase>() { modelBase };
            Header = modelBase.Header;
            Icon = new BitmapImage(modelBase.Icon);
        }
    }

    public abstract class ContentViewModelBase<T> {
        public virtual ObservableCollection<T> ItemsSource { get; protected set; }
        protected IDataProvider DataProvider { get; private set; }
        protected IList<T> Items { get; private set; }
        protected ModuleType ModuleType { get; private set; }

        protected ContentViewModelBase(ModuleType moduleType) {
            ModuleType = moduleType;
            InitializeDataProvider();
            InitializeItems();
            UpdateItemsSource();
            if(this.IsInDesignMode())
                InitializeInDesignMode();
            else
                Messenger.Default.Send(new ContentViewModelInjectedMessage(ModuleType, this));
        }

        protected virtual void InitializeInDesignMode() { }
        protected virtual void InitializeItems() {
            Items = DataProvider.GetItems<T>().ToList();
        }
        protected virtual void InitializeDataProvider() {
            if(this.IsInDesignMode())
                DataProvider = new DesignTimeDataProvider();
            else
                DataProvider = DataSource.GetDefaultDataProvider();
        }
        protected virtual void UpdateItemsSource() {
            ItemsSource = new ObservableCollection<T>(Items);
        }
    }
    public class PrefixEnumWithExternalMetadata {
        public static void BuildMetadata(EnumMetadataBuilder<PersonPrefix> builder) {
            builder
                .Member(PersonPrefix.Dr)
                    .ImageUri("pack://application:,,,/DevExpress.MailClient.Xpf;component/Images/Tasks/Doctor.png")
                .EndMember()
                .Member(PersonPrefix.Mr)
                    .ImageUri("pack://application:,,,/DevExpress.MailClient.Xpf;component/Images/Tasks/Mr.png")
                .EndMember()
                .Member(PersonPrefix.Ms)
                    .ImageUri("pack://application:,,,/DevExpress.MailClient.Xpf;component/Images/Tasks/Ms.png")
                .EndMember()
                .Member(PersonPrefix.Miss)
                    .ImageUri("pack://application:,,,/DevExpress.MailClient.Xpf;component/Images/Tasks/Miss.png")
                .EndMember()
                .Member(PersonPrefix.Mrs)
                    .ImageUri("pack://application:,,,/DevExpress.MailClient.Xpf;component/Images/Tasks/Mrs.png")
                .EndMember();
        }
    }
}