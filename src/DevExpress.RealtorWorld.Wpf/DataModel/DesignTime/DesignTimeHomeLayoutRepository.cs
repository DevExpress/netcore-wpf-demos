using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace DevExpress.RealtorWorld.Xpf.DataModel{
    public class DesignTimeHomeLayoutRepository : IHomeLayoutRepository {
        static List<HomeLayout> homeLayouts;

        IQueryable<HomeLayout> IRepository<HomeLayout, int>.Get() { return GetSampleData(); }
        ObservableCollection<HomeLayout> IRepository<HomeLayout, int>.Local { get { return new ObservableCollection<HomeLayout>(GetSampleData()); } }
        HomeLayout IRepository<HomeLayout, int>.Find(int id) {
            return GetSampleData().Where(h => h.ID == id).FirstOrDefault();
        }
        IUnitOfWork IRepository<HomeLayout, int>.UnitOfWork { get { return null; } }

        static IQueryable<HomeLayout> GetSampleData() {
            if(homeLayouts != null) return homeLayouts.AsQueryable();
            homeLayouts = new List<HomeLayout>();
            byte[] photoStub = null;
            using(var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.RealtorWorld.Xpf.Images.HomeLayout.jpg")) {
                photoStub = new byte[(int)stream.Length];
                stream.Read(photoStub, 0, photoStub.Length);
            }
            for(int i = 0; i < 5; ++i)
                homeLayouts.Add(new HomeLayout() { ID = i, Image = photoStub });
            return homeLayouts.AsQueryable();
        }
    }
}
