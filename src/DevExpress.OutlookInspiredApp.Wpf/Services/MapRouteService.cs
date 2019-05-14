using DevExpress.Mvvm.UI;
using DevExpress.DevAV.ViewModels;
using DevExpress.Xpf.Map;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExpress.DevAV.ViewModels {
    public interface IMapRouteService {
        Task<RouteCalculationResult> CalculateRouteAsync(IEnumerable<RouteWaypoint> waypoints, DistanceMeasureUnit unit, BingRouteOptimization optimization, BingTravelMode mode);
    }
}

namespace DevExpress.DevAV {
    public class MapRouteService : ServiceBase, IMapRouteService {
        InformationLayer Layer { get { return (InformationLayer)AssociatedObject; } }
        BingRouteDataProvider Provider { get { return (BingRouteDataProvider)Layer.DataProvider; } }
        TaskCompletionSource<RouteCalculationResult> taskSource;
        public Task<RouteCalculationResult> CalculateRouteAsync(IEnumerable<RouteWaypoint> waypoints, DistanceMeasureUnit unit, BingRouteOptimization optimization, BingTravelMode mode) {
            taskSource = new TaskCompletionSource<RouteCalculationResult>();
            Provider.RouteOptions = new BingRouteOptions { Mode = mode, DistanceUnit = unit, RouteOptimization = optimization };
            Provider.CalculateRoute(waypoints.ToList());
            Provider.RouteCalculated += Provider_RouteCalculated;
            return taskSource.Task;
        }
        void Provider_RouteCalculated(object sender, BingRouteCalculatedEventArgs e) {
            Provider.RouteCalculated -= Provider_RouteCalculated;
            if(e.Cancelled)
                taskSource.SetCanceled();
            else if(e.Error != null)
                taskSource.SetException(e.Error);
            else
                taskSource.SetResult(e.CalculationResult);
            
        }
    }
}
