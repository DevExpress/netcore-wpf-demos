using DevExpress.DevAV.ViewModels;
using DevExpress.Xpf.Map;
using System.Windows.Controls;

namespace DevExpress.DevAV.Views {
    public partial class OrderMapView : UserControl {
        public OrderMapView() {
            InitializeComponent();
        }
        void BingGeocodeDataProvider_LayerItemsGenerating(object sender, LayerItemsGeneratingEventArgs args) {
            foreach(var pushpinItem in args.Items) {
                MapPushpin pushpin = pushpinItem as MapPushpin;
                if(pushpin != null) 
                    ((INavigatorMapViewModel)DataContext).NewPushpinCreated(pushpin);                
            }
        }
        void BingRouteDataProvider_LayerItemsGenerating(object sender, LayerItemsGeneratingEventArgs args) {
            if((args.Items).Length < 3)
                return;
            ((MapPushpin)args.Items[1]).Text = "A";
            ((MapPushpin)args.Items[2]).Text = "B";
            mapControl.ZoomToRegion(((MapPushpin)args.Items[1]).Location, ((MapPushpin)args.Items[2]).Location, 0.4);
        }
    }
}
