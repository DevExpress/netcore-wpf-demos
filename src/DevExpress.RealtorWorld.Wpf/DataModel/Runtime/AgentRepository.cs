using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace DevExpress.RealtorWorld.Xpf.DataModel {
    public class AgentRepository : XmlRepository<Agent, int>, IAgentRepository {
        protected override int GetKey(Agent entity) { return entity.ID; }
        protected override ReusableStream GetDataStream() {
            return GetDataStreamCore("Homes.xml");
        }
        protected override Type ObservableCollectionType { get { return typeof(EntityObservableCollection); } }
        [XmlRoot("dsHomes", Namespace = "http://tempuri.org/dsHomes.xsd")]
        public class EntityObservableCollection : ObservableCollection<Agent> { }
    }
}
