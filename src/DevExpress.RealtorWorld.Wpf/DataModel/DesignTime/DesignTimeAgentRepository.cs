using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace DevExpress.RealtorWorld.Xpf.DataModel {
    public class DesignTimeAgentRepository : IAgentRepository {
        static List<Agent> agents;

        IQueryable<Agent> IRepository<Agent, int>.Get() { return GetSampleData(); }
        ObservableCollection<Agent> IRepository<Agent, int>.Local { get { return new ObservableCollection<Agent>(GetSampleData()); } }
        Agent IRepository<Agent, int>.Find(int id) {
            return agents.Where(a => a.ID == id).FirstOrDefault();
        }
        IUnitOfWork IRepository<Agent, int>.UnitOfWork { get { return null; } }

        static IQueryable<Agent> GetSampleData() {
            if(agents != null) return agents.AsQueryable();
            byte[] photoStub = null;
            using(var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.RealtorWorld.Xpf.Images.AgentPhoto.png")) {
                photoStub = new byte[(int)stream.Length];
                stream.Read(photoStub, 0, photoStub.Length);
            }
            agents = new List<Agent>();
            for(int id = 0; id < 6; ++id)
                agents.Add(new Agent() { ID = id + 1, Photo = photoStub, FirstName = "Vernon", LastName = "Rounds", Email = "vernon_rounds@dxrealtor.com", Phone = "(555)682-5403" });
            return agents.AsQueryable();
        }
    }
}
