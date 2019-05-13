using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using DevExpress.RealtorWorld.Xpf.DataModel;
using DevExpress.RealtorWorld.Xpf.Helpers;
using DevExpress.Mvvm;

namespace DevExpress.RealtorWorld.Xpf.ViewModel {
    public sealed class HomeRepositoryViewModel : ViewModel, IDocumentContent {
        static int? savedSelectedHomeID;
        readonly IUnitOfWorkFactory unitOfWorkFactory;
        IRealtorWorldUnitOfWork unitOfWork;
        Home selectedHome;
        ReadOnlyCollection<HomePhoto> selectedHomePhotos;
        HomeLayout selectedHomeLayout;
        Agent selectedHomeAgent;

        public HomeRepositoryViewModel() : this(UnitOfWorkSource.GetUnitOfWorkFactory(IsInDesignMode)) { }
        public HomeRepositoryViewModel(IUnitOfWorkFactory unitOfWorkFactory) {
            this.unitOfWorkFactory = unitOfWorkFactory;
            Refresh();
        }
        IDocumentOwner IDocumentContent.DocumentOwner { get; set; }
        object IDocumentContent.Title { get { return null; } }
        void IDocumentContent.OnClose(CancelEventArgs e) {
            savedSelectedHomeID = SelectedHome == null ? (int?)null : SelectedHome.ID;
        }
        void IDocumentContent.OnDestroy() { }

        public IEnumerable<Home> Entities {
            get {
                unitOfWork.Homes.Get().Load();
                return unitOfWork.Homes.Local;
            }
        }
        public Home SelectedHome {
            get { return selectedHome; }
            set { SetProperty(ref selectedHome, value, () => SelectedHome, OnSelectedHomeChanged); }
        }
        public ReadOnlyCollection<HomePhoto> SelectedHomePhotos {
            get { return selectedHomePhotos; }
            private set { SetProperty(ref selectedHomePhotos, value, () => SelectedHomePhotos); }
        }
        public Agent SelectedHomeAgent {
            get { return selectedHomeAgent; }
            private set { SetProperty(ref selectedHomeAgent, value, () => SelectedHomeAgent); }
        }
        public HomeLayout SelectedHomeLayout {
            get { return selectedHomeLayout; }
            private set { SetProperty(ref selectedHomeLayout, value, () => SelectedHomeLayout); }
        }

        protected override void OnParameterChanged(object parameter) {
            base.OnParameterChanged(parameter);
            int? homeID = parameter as int?;
            if(homeID == null) return;
            SelectedHome = unitOfWork.Homes.Find(homeID.Value);
        }
        void OnSelectedHomeChanged() {
            SelectedHomePhotos = SelectedHome == null ? null : unitOfWork.HomePhotos.Get().Where(p => p.ParentID == SelectedHome.HomePhotoID).ToList().AsReadOnly();
            SelectedHomeAgent = SelectedHome == null ? null : unitOfWork.Agents.Get().Where(a => a.ID == SelectedHome.AgentID).FirstOrDefault();
            SelectedHomeLayout = SelectedHome == null ? null : unitOfWork.HomeLayouts.Find(SelectedHome.HomeLayoutID);
        }
        void Refresh() {
            unitOfWork = unitOfWorkFactory.CreateUnitOfWork();
            RaisePropertyChanged(() => Entities);
            if(SelectedHome == null)
                SelectedHome = savedSelectedHomeID == null ? Entities.FirstOrDefault() : unitOfWork.Homes.Find(savedSelectedHomeID.Value);
            savedSelectedHomeID = null;
        }
    }
}
