using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using DevExpress.RealtorWorld.Xpf.DataModel;
using DevExpress.RealtorWorld.Xpf.Helpers;
using DevExpress.Mvvm;

namespace DevExpress.RealtorWorld.Xpf.ViewModel {
    public class AgentRepositoryViewModel : ViewModel, IDocumentContent {
        static decimal? savedYearRangeMinValue;
        static decimal? savedYearRangeMaxValue;
        static int? savedSelectedAgentID;
        decimal yearRangeMinValue;
        decimal yearRangeMaxValue;
        readonly IUnitOfWorkFactory unitOfWorkFactory;
        IRealtorWorldUnitOfWork unitOfWork;
        Agent selectedAgent;
        ReadOnlyCollection<Home> selectedAgentHomes;
        string selectedAgentFullName;
        AgentStatisticData selectedAgentStatisticData;
        Home selectedHome;
        

        public AgentRepositoryViewModel()
            : this(UnitOfWorkSource.GetUnitOfWorkFactory(IsInDesignMode)) {
        }
        public AgentRepositoryViewModel(IUnitOfWorkFactory unitOfWorkFactory) {
            this.unitOfWorkFactory = unitOfWorkFactory;
            Refresh();
            YearRangeMaxValue = savedYearRangeMaxValue == null ? 2011.6M : (decimal)savedYearRangeMaxValue;
            YearRangeMinValue = savedYearRangeMinValue == null ? YearRangeMaxValue - 3.2M : (decimal)savedYearRangeMinValue;
        }
        IDocumentOwner IDocumentContent.DocumentOwner { get; set; }
        object IDocumentContent.Title { get { return null; } }
        void IDocumentContent.OnClose(CancelEventArgs e) {
            savedYearRangeMaxValue = YearRangeMaxValue;
            savedYearRangeMinValue = YearRangeMinValue;
            savedSelectedAgentID = SelectedAgent == null ? (int?)null : SelectedAgent.ID;
        }
        void IDocumentContent.OnDestroy() { }

        public IEnumerable<Agent> Entities {
            get {
                unitOfWork.Agents.Get().Load();
                return unitOfWork.Agents.Local;
            }
        }
        public Agent SelectedAgent {
            get { return selectedAgent; }
            set { SetProperty(ref selectedAgent, value, () => SelectedAgent, OnSelectedAgentChanged); }
        }
        public ReadOnlyCollection<Home> SelectedAgentHomes {
            get { return selectedAgentHomes; }
            private set { SetProperty(ref selectedAgentHomes, value, () => SelectedAgentHomes); }
        }
        public string SelectedAgentFullName {
            get { return selectedAgentFullName; }
            private set { SetProperty(ref selectedAgentFullName, value, () => SelectedAgentFullName); }
        }
        public Home SelectedHome {
            get { return selectedHome; }
            set { SetProperty(ref selectedHome, value, () => SelectedHome); }
        }
        public AgentStatisticData SelectedAgentStatisticData {
            get { return selectedAgentStatisticData; }
            private set { SetProperty(ref selectedAgentStatisticData, value, () => SelectedAgentStatisticData); }
        }
        public decimal YearRangeMinValue {
            get { return yearRangeMinValue; }
            set { SetProperty(ref yearRangeMinValue, value, () => YearRangeMinValue); }
        }
        public decimal YearRangeMaxValue {
            get { return yearRangeMaxValue; }
            set { SetProperty(ref yearRangeMaxValue, value, () => YearRangeMaxValue); }
        }        

        protected override void OnParameterChanged(object parameter) {
            base.OnParameterChanged(parameter);
            int? agentID = parameter as int?;
            if(agentID == null) return;
            SelectedAgent = unitOfWork.Agents.Find(agentID.Value);
        }
        void OnSelectedAgentChanged() {
            SelectedAgentHomes = SelectedAgent == null ? null : unitOfWork.Homes.Get().Where(h => h.AgentID == SelectedAgent.ID).ToList().AsReadOnly();
            SelectedAgentFullName = SelectedAgent == null ? string.Empty : SelectedAgent.FirstName + " " + SelectedAgent.LastName;
            SelectedAgentStatisticData = SelectedAgent == null ? null : unitOfWork.AgentStatisticData.Find(SelectedAgent.AgentStatisticDataID);
        }
        void Refresh() {
            unitOfWork = unitOfWorkFactory.CreateUnitOfWork();
            RaisePropertyChanged(() => Entities);
            if(SelectedAgent == null)
                SelectedAgent = savedSelectedAgentID == null ? Entities.FirstOrDefault() : unitOfWork.Agents.Find(savedSelectedAgentID.Value);
            savedSelectedAgentID = null;
        }
    }
}