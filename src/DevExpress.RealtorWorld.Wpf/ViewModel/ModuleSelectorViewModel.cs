using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.RealtorWorld.Xpf.DataModel;

namespace DevExpress.RealtorWorld.Xpf.ViewModel {
    public sealed class ModuleSelectorViewModel : ViewModel {
        readonly IUnitOfWorkFactory unitOfWorkFactory;
        IRealtorWorldUnitOfWork unitOfWork;
        IEnumerable<HomeWithPhoto> homeRepositoryTileDataSource;
        HomeWithPhoto homeRepositoryTileData;
        HomeWithPhoto nextHomeRepositoryTileData;
        bool animateHomeRepositoryTileContent;
        IEnumerator<HomeWithPhoto> homeRepositoryTileDataEnumerator;
        IEnumerable<Agent> agentRepositoryTileDataSource;
        Agent agentRepositoryTileData;
        Agent nextAgentRepositoryTileData;
        bool animateAgentRepositoryTileContent;
        IEnumerator<Agent> agentRepositoryTileDataEnumerator;

        public ModuleSelectorViewModel() : this(UnitOfWorkSource.GetUnitOfWorkFactory(IsInDesignMode)) { }
        public ModuleSelectorViewModel(IUnitOfWorkFactory unitOfWorkFactory) {
            this.unitOfWorkFactory = unitOfWorkFactory;
            Refresh();
        }
        public IEnumerable<HomeWithPhoto> HomeRepositoryTileDataSource {
            get { return homeRepositoryTileDataSource; }
            private set { SetProperty(ref homeRepositoryTileDataSource, value, () => HomeRepositoryTileDataSource); }
        }
        public IEnumerable<Agent> AgentRepositoryTileDataSource {
            get { return agentRepositoryTileDataSource; }
            private set { SetProperty(ref agentRepositoryTileDataSource, value, () => AgentRepositoryTileDataSource); }
        }
        public bool AnimateHomeRepositoryTileContent {
            get { return animateHomeRepositoryTileContent; }
            private set { SetProperty(ref animateHomeRepositoryTileContent, value, () => AnimateHomeRepositoryTileContent); }
        }
        public bool AnimateAgentRepositoryTileContent {
            get { return animateAgentRepositoryTileContent; }
            private set { SetProperty(ref animateAgentRepositoryTileContent, value, () => AnimateAgentRepositoryTileContent); }
        }

        void Refresh() {
            unitOfWork = unitOfWorkFactory.CreateUnitOfWork();
            LoadNextHomeRepositoryTileData();
            LoadNextAgentRepositoryTileData();
        }
        IEnumerable<HomeWithPhoto> HomeRepositoryTileDataSourceCore {
            get {
                while(true) {
                    if(this.nextHomeRepositoryTileData != null) {
                        this.homeRepositoryTileData = this.nextHomeRepositoryTileData;
                        LoadNextHomeRepositoryTileData();
                    }
                    yield return this.homeRepositoryTileData;
                }
            }
        }
        IEnumerable<Agent> AgentRepositoryTileDataSourceCore {
            get {
                while(true) {
                    if(this.nextAgentRepositoryTileData != null) {
                        this.agentRepositoryTileData = this.nextAgentRepositoryTileData;
                        LoadNextAgentRepositoryTileData();
                    }
                    yield return this.agentRepositoryTileData;
                }
            }
        }
        void LoadNextHomeRepositoryTileData() {
            this.nextHomeRepositoryTileData = null;
            Task.Factory.StartNew(() => {
                if(homeRepositoryTileDataEnumerator == null || !homeRepositoryTileDataEnumerator.MoveNext()) {
                    homeRepositoryTileDataEnumerator = GetHomeRepositoryTileDataEnumerator();
                    if(!homeRepositoryTileDataEnumerator.MoveNext()) return;
                }
                this.nextHomeRepositoryTileData = homeRepositoryTileDataEnumerator.Current;
                if(HomeRepositoryTileDataSource == null) {
                    HomeRepositoryTileDataSource = HomeRepositoryTileDataSourceCore;
                    AnimateHomeRepositoryTileContent = true;
                }
            });
        }
        void LoadNextAgentRepositoryTileData() {
            this.nextAgentRepositoryTileData = null;
            Task.Factory.StartNew(() => {
                if(agentRepositoryTileDataEnumerator == null || !agentRepositoryTileDataEnumerator.MoveNext()) {
                    agentRepositoryTileDataEnumerator = GetAgentRepositoryTileDataEnumerator();
                    if(!agentRepositoryTileDataEnumerator.MoveNext()) return;
                }
                this.nextAgentRepositoryTileData = agentRepositoryTileDataEnumerator.Current;
                if(AgentRepositoryTileDataSource == null) {
                    AgentRepositoryTileDataSource = AgentRepositoryTileDataSourceCore;
                    AnimateAgentRepositoryTileContent = true;
                }
            });
        }
        IEnumerator<HomeWithPhoto> GetHomeRepositoryTileDataEnumerator() {
            foreach(Home home in unitOfWork.Homes.Get().ToArray()) {
                yield return new HomeWithPhoto() { Home = home, Image = home.Photo };
                int homePhotoID = home.HomePhotoID;
                foreach(HomePhoto homePhoto in unitOfWork.HomePhotos.Get().Where(p => p.ParentID == homePhotoID).ToArray()) {
                    yield return new HomeWithPhoto() { Home = home, Image = homePhoto.Photo };
                }
            }
        }
        IEnumerator<Agent> GetAgentRepositoryTileDataEnumerator() {
            foreach(Agent agent in unitOfWork.Agents.Get().ToArray()) {
                yield return agent;
            }
        }
    }
    public class HomeWithPhoto {
        public Home Home { get; set; }
        public byte[] Image { get; set; }
    }
}