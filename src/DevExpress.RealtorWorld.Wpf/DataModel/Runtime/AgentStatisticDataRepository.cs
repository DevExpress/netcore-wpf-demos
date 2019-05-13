using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace DevExpress.RealtorWorld.Xpf.DataModel {
    public class AgentStatisticDataRepository : LazyRepository<AgentStatisticData, int>, IAgentStatisticDataRepository {
        const int Count = 6;
        static readonly string[] Regions = new string[] { "North-East", "Mid-West", "South", "West" };
        static Random random = new Random();
        static AgentStatisticData[] layouts = new AgentStatisticData[Count];
        static object[] layoutLocks;

        static AgentStatisticDataRepository() {
            layoutLocks = Enumerable.Range(0, Count).Select(i => new object()).ToArray();
        }
        protected override AgentStatisticData FindCore(int id) {
            lock(layoutLocks[id]) {
                if(layouts[id] != null) return layouts[id];
                layouts[id] = new AgentStatisticData();
                List<AgentStatisticDataPoint> points = new List<AgentStatisticDataPoint>();
                int year = DateTime.Now.Year;
                int baseValue = 0;
                foreach(string region in Regions) {
                    baseValue += 5;
                    for(int i = 0; i < 10; i++) {
                        AgentStatisticDataPoint point = new AgentStatisticDataPoint() { Region = region };
                        point.Year = year - i;
                        point.Value = random.Next(baseValue);
                        points.Add(point);
                    }
                }
                layouts[id].Points = points.AsQueryable();
                return layouts[id];
            }
        }
        protected override IEnumerable<AgentStatisticData> GetData() {
            for(int i = 0; i < Count; ++i) {
                yield return FindCore(i);
            }
        }
    }
}
