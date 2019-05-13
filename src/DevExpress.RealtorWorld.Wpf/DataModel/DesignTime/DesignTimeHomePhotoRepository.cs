using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace DevExpress.RealtorWorld.Xpf.DataModel{
    public class DesignTimeHomePhotoRepository : IHomePhotoRepository {
        static List<HomePhoto> homePhotos;

        IQueryable<HomePhoto> IRepository<HomePhoto, int>.Get() { return GetSampleData(); }
        ObservableCollection<HomePhoto> IRepository<HomePhoto, int>.Local { get { return new ObservableCollection<HomePhoto>(GetSampleData()); } }
        HomePhoto IRepository<HomePhoto, int>.Find(int id) {
            return homePhotos.Where(h => h.ID == id).FirstOrDefault();
        }
        IUnitOfWork IRepository<HomePhoto, int>.UnitOfWork { get { return null; } }

        static IQueryable<HomePhoto> GetSampleData() {
            if(homePhotos != null) return homePhotos.AsQueryable();
            homePhotos = new List<HomePhoto>();
            byte[] photoStub = null;
            using(var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.RealtorWorld.Xpf.Images.HomePhoto.jpg")) {
                photoStub = new byte[(int)stream.Length];
                stream.Read(photoStub, 0, photoStub.Length);
            }
            for(int i = 0; i < 30; ++i)
                homePhotos.Add(new HomePhoto() { ID = i + 1, ParentID = i / 3 + 1, Photo = photoStub });
            return homePhotos.AsQueryable();
        }
    }
}
