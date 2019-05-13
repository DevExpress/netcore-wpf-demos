using DevExpress.DevAV.ViewModels;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;
using System.Windows;

namespace DevExpress.DevAV.Common.View {
    public class DetailViewSelectorBehavior : Behavior<GridControl> {
        public DetailViewSelectorBehavior() { }

        public static readonly DependencyProperty ViewKindProperty =
            DependencyProperty.Register("ViewKind", typeof(CollectionViewKind), typeof(DetailViewSelectorBehavior),
                new PropertyMetadata(CollectionViewKind.CardView, (d, e) => ((DetailViewSelectorBehavior)d).UpdateViewKind()));

        public CollectionViewKind ViewKind { get { return (CollectionViewKind)GetValue(ViewKindProperty); } set { SetValue(ViewKindProperty, value); } }

        public GridViewBase CardView { get; set; }
        public GridViewBase TableView { get; set; }

        void UpdateViewKind() {
            if(AssociatedObject == null || CardView == null || TableView == null || ViewKind == CollectionViewKind.Carousel)
                return;
            AssociatedObject.View = ViewKind == CollectionViewKind.CardView ? CardView : TableView;
        }
        protected override void OnAttached() {
            base.OnAttached();
            UpdateViewKind();
        }
        protected override void OnDetaching() {
            base.OnDetaching();
        }
    }
}
