using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Ribbon;

namespace DevExpress.MailClient.View {
    public class ThemeSelectorBehavior : RibbonGalleryItemThemeSelectorBehavior {
        protected override ICollectionView CreateCollectionView() {
            var view = CollectionViewSource.GetDefaultView(
                Theme.Themes.Where(x => x.ShowInThemeSelector && !IsThemeExcluded(x)).Select(y => new ThemeViewModel(y)).ToArray());
            view.GroupDescriptions.Add(new PropertyGroupDescription("Theme.Category"));
            return view;
        }
        static bool IsThemeExcluded(Theme theme) {
            var name = theme.Name.ToLowerInvariant();
            return name.StartsWith("office2013")
                 || (name.StartsWith("office2016") && !name.EndsWith("se"))
                 || name == "deepblue"
                 || name.Contains("touch");
        }
    }
}
