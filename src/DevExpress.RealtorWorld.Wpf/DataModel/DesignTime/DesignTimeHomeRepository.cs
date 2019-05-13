using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace DevExpress.RealtorWorld.Xpf.DataModel{
    public class DesignTimeHomeRepository : IHomeRepository {
        static List<Home> homes;

        IQueryable<Home> IRepository<Home, int>.Get() { return GetSampleData(); }
        ObservableCollection<Home> IRepository<Home, int>.Local { get { return new ObservableCollection<Home>(GetSampleData()); } }
        Home IRepository<Home, int>.Find(int id) {
            return homes.Where(h => h.ID == id).FirstOrDefault();
        }
        IUnitOfWork IRepository<Home, int>.UnitOfWork { get { return null; } }

        static IQueryable<Home> GetSampleData() {
            if(homes != null) return homes.AsQueryable();
            byte[] photoStub = null;
            using(var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.RealtorWorld.Xpf.Images.Home.jpg")) {
                photoStub = new byte[(int)stream.Length];
                stream.Read(photoStub, 0, photoStub.Length);
            }
            homes = new List<Home>();
            for(int id = 0; id < 100; ++id)
                homes.Add(new Home() {
                    ID = id, Photo = photoStub, YearBuilt = "2000", Beds = 4, Baths = 4,
                    HouseSize = 7840, LotSize = 0.3, Price = 610000, Address = "13673 Pearl Dr #7, Monroe, MI 48161",
                    Features = "Dishwasher, Disposal, Separate laundry room"
                });
            return homes.AsQueryable();
        }
    }
}
