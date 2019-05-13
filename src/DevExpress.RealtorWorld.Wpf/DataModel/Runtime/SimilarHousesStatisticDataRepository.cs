using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DevExpress.RealtorWorld.Xpf.DataModel {
    public class SimilarHousesStatisticDataRepository : LazyRepository<SimilarHousesStatisticData, int>, ISimilarHousesStatisticDataRepository {
        const int Count = 23;
        static Random random = new Random();
        static SimilarHousesStatisticData[] layouts = new SimilarHousesStatisticData[Count];
        static object[] layoutLocks = new object[Count];

        static SimilarHousesStatisticDataRepository() {
            layoutLocks = Enumerable.Range(0, Count).Select(i => new object()).ToArray();
        }

        protected override SimilarHousesStatisticData FindCore(int id) {
            lock(layoutLocks[id]) {
                if(layouts[id] != null) return layouts[id];
                layouts[id] = new SimilarHousesStatisticData();
                List<SimilarHousesStatisticDataPoint> points = new List<SimilarHousesStatisticDataPoint>();
                int year = DateTime.Now.Year;
                for(int i = 9; i >= 0; i--) {
                    SimilarHousesStatisticDataPoint point = new SimilarHousesStatisticDataPoint();
                    point.Year = year - i;
                    point.ProposalCount = random.Next(50, 250);
                    point.SoldCount = point.ProposalCount * random.Next(10, 80) / 100;
                    points.Add(point);
                }
                layouts[id].Points = points.AsQueryable();
                return layouts[id];
            }
        }
        protected override IEnumerable<SimilarHousesStatisticData> GetData() {
            for(int i = 0; i < Count; ++i) {
                yield return FindCore(i);
            }
        }
    }
}
