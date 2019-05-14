using DevExpress.Mvvm.UI;
using DevExpress.DevAV.ViewModels;
using DevExpress.Xpf.Map;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExpress.DevAV.ViewModels {
    public interface IMapPushpinsService {
        void Clear();
    }
}

namespace DevExpress.DevAV {
    public class MapPushpinsService : ServiceBase, IMapPushpinsService {
        MapControl Map { get { return (MapControl)AssociatedObject; } }
        IEnumerable<InformationLayer> InformationLayers { get { return Map.Layers.Where(l => l is InformationLayer).Cast<InformationLayer>(); } }
        public void Clear() {
            foreach(var layer in InformationLayers) {
                layer.ClearResults();
            }
        }
    }
}
