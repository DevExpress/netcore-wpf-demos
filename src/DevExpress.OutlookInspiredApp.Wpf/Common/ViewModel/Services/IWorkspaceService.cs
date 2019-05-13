using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DevExpress.DevAV.Common.ViewModel {
    public class Workspace {
        Dictionary<string, string> regions = new Dictionary<string, string>();

        public void AddRegion(string regionId, string regionLayout) {
            regions.Add(regionId, regionLayout);
        }
        public string FindRegionLayout(string regionId) {
            string regionLayout = null;
            return regions.TryGetValue(regionId, out regionLayout) ? regionLayout : null;
        }
        public IEnumerable<KeyValuePair<string, string>> Regions { get { return regions; } }
    }
    public interface IWorkspaceService {
        Workspace SaveWorkspace();
        void RestoreWorkspace(Workspace workspace);
    }
}