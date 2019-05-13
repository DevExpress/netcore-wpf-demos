using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace DevExpress.RealtorWorld.Xpf.DataModel {
    public class HomePhotoRepository : XmlRepository<HomePhoto, int>, IHomePhotoRepository {
        protected override int GetKey(HomePhoto entity) { return entity.ID; }
        protected override ReusableStream GetDataStream() {
            return GetDataStreamCore("HomePhotos.xml");
        }
        protected override Type ObservableCollectionType { get { return typeof(EntityObservableCollection); } }
        [XmlRoot("dsPhotos", Namespace = "http://tempuri.org/dsPhotos.xsd")]
        public class EntityObservableCollection : ObservableCollection<HomePhoto> { }
    }
}
