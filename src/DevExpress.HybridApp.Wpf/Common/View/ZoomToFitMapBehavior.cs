using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Map;

namespace DevExpress.DevAV.Common.View {
    public class ZoomToFitMapBehavior : Behavior<MapControl> {
        public ZoomToFitMapBehavior() {
            mapVectorLayers = new List<VectorLayer>();
        }
        List<VectorLayer> mapVectorLayers;
        bool zoomValueUpdated = false;

        public string ZoomLayerName { get; set; }
        public double PaddingFactor { get; set; }

        protected override void OnAttached() {
            base.OnAttached();
            this.AssociatedObject.Loaded += MapControlLoaded;
        }
        protected override void OnDetaching() {
            this.AssociatedObject.Loaded -= MapControlLoaded;
            base.OnDetaching();
        }

        void MapControlLoaded(object sender, RoutedEventArgs e) {
            mapVectorLayers.Clear();
            foreach(var layer in this.AssociatedObject.Layers) {
                VectorLayer vectorLayer = layer as VectorLayer;
                if(vectorLayer != null && vectorLayer.Data != null && (string.IsNullOrEmpty(ZoomLayerName) ? true : vectorLayer.Name == ZoomLayerName)) {
                    mapVectorLayers.Add(vectorLayer);
                    vectorLayer.DataLoaded -= OnDataLoaded;
                    vectorLayer.DataLoaded += OnDataLoaded;
                }
            }
        }
        void OnDataLoaded(object sender, DataLoadedEventArgs e) {
            var displayItems = ((VectorLayer)sender).Data.DisplayItems;
            if(displayItems == null)
                return; 
            if(Enumerable.Count(displayItems) > 3 && !zoomValueUpdated) {
                zoomValueUpdated = true;
                ZoomToFit();
            }
        }
        void ZoomToFit() {
            this.AssociatedObject.ZoomToFitLayerItems(mapVectorLayers, PaddingFactor);
        }
    }
}
