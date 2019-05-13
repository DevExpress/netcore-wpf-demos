using System;
using System.Collections.Generic;
using System.Linq;

namespace DevExpress.RealtorWorld.Xpf.DataModel {
    public class HomeLayoutRepository : LazyRepository<HomeLayout, int>, IHomeLayoutRepository {
        const int Count = 5;
        static HomeLayout[] layouts = new HomeLayout[Count];
        static object[] layoutLocks = new object[Count];

        static HomeLayoutRepository() {
            layoutLocks = Enumerable.Range(0, Count).Select(i => new object()).ToArray();
        }

        protected override HomeLayout FindCore(int id) {
            lock(layoutLocks[id]) {
                if(layouts[id] != null) return layouts[id];
                using(ReusableStream s = DataHelper.GetDataFile(string.Format("Images\\HomePlan{0}.jpg", id + 1))) {
                    byte[] data = new byte[(int)s.Data.Length];
                    s.Data.Read(data, 0, data.Length);
                    layouts[id] = new HomeLayout() { ID = id, Image = data };
                }
                return layouts[id];
            }
        }
        protected override IEnumerable<HomeLayout> GetData() {
            for(int i = 0; i < Count; ++i) {
                yield return FindCore(i);
            }
        }
    }
}
