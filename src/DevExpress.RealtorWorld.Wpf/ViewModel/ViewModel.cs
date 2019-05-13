using System;
using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.RealtorWorld.Xpf.Helpers;

namespace DevExpress.RealtorWorld.Xpf.ViewModel {
    public class ViewModel : ViewModelBase {
        ICommand onViewLoadedCommand;
        ICommand navigateCommand;
        ICommand navigateToHomeCommand;
        ICommand navigateToAgentCommand;

        public ICommand OnViewLoadedCommand {
            get {
                if(onViewLoadedCommand == null)
                    onViewLoadedCommand = new DelegateCommand(OnViewLoaded);
                return onViewLoadedCommand;
            }
        }
        public ICommand NavigateCommand {
            get {
                if(navigateCommand == null)
                    navigateCommand = new DelegateCommand<string>(Navigate);
                return navigateCommand;
            }
        }
        public ICommand NavigateToHomeCommand {
            get {
                if(navigateToHomeCommand == null)
                    navigateToHomeCommand = new DelegateCommand<int?>(NavigateHomeRepositoryView, id => id != null);
                return navigateToHomeCommand;
            }
        }
        public ICommand NavigateToAgentCommand {
            get {
                if(navigateToAgentCommand == null)
                    navigateToAgentCommand = new DelegateCommand<int?>(NavigateAgentRepositoryView, id => id != null);
                return navigateToAgentCommand;
            }
        }
        void NavigateHomeRepositoryView(int? id) {
            NavigationService.Navigate("HomeRepositoryView", id, this);
        }
        void NavigateAgentRepositoryView(int? id) {
            NavigationService.Navigate("AgentRepositoryView", id, this);
        }
        public void Navigate(string target) {
            NavigationService.Navigate(target, null, this);
        }
        protected INavigationService NavigationService { get { return GetService<INavigationService>(); } }
        protected virtual void OnViewLoaded() { }
    }
}