using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DevExpress.RealtorWorld.Xpf.DataModel {
    public class HomePriceStatisticDataRepository : LazyRepository<HomePriceStatisticData, HomePriceStatisticDataKey>, IHomePriceStatisticDataRepository {
        const int Count = 23;
        static Random random = new Random();
        static HomePriceStatisticData[] layouts = new HomePriceStatisticData[Count];
        static object[] layoutLocks = new object[Count];

        static HomePriceStatisticDataRepository() {
            layoutLocks = Enumerable.Range(0, Count).Select(i => new object()).ToArray();
        }

        protected override HomePriceStatisticData FindCore(HomePriceStatisticDataKey key) {
            lock(layoutLocks[key.ID]) {
                if(layouts[key.ID] != null) return layouts[key.ID];
                layouts[key.ID] = new HomePriceStatisticData();
                List<HomePriceStatisticDataPoint> points = new List<HomePriceStatisticDataPoint>();
                DateTime beginDate = DateTime.Now;
                DateTime endDate = beginDate - new TimeSpan(500, 0, 0, 0, 0);
                decimal value = key.Price / 1000M;
                for(DateTime date = beginDate; date > endDate; date = date - new TimeSpan(1, 0, 0, 0, 0)) {
                    points.Add(new HomePriceStatisticDataPoint() { Date = date, Value = value });
                    value *= (decimal)(1 + (random.NextDouble() - 0.5) / 1000);
                }
                layouts[key.ID].Points = points.AsQueryable();
                return layouts[key.ID];
            }
        }
        protected override IEnumerable<HomePriceStatisticData> GetData() {
            throw new NotSupportedException();
        }
    }
}
