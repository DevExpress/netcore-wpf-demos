using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;
using System.Windows;

namespace DevExpress.MailClient.Behaviors {
    public class GridUpdateLockerBehavior : Behavior<GridControl> {
        public static readonly DependencyProperty IsLockedProperty = 
            DependencyProperty.Register("IsLocked", typeof(bool), typeof(GridUpdateLockerBehavior),
            new PropertyMetadata(false, (d, e) => ((GridUpdateLockerBehavior)d).OnIsLockedChanged()));

        public bool IsLocked {
            get { return (bool)GetValue(IsLockedProperty); }
            set { SetValue(IsLockedProperty, value); }
        }

        bool isGridLocked = false;

        void OnIsLockedChanged() {
            UpdateGridLock();
        }
        protected override void OnAttached() {
            base.OnAttached();
            UpdateGridLock();
        }
        protected override void OnDetaching() {
            UpdateGridLock();
            base.OnDetaching();
        }
        void UpdateGridLock() {
            bool needLock = IsLocked && IsAttached;
            if(needLock && !isGridLocked) {
                isGridLocked = true;
                AssociatedObject.Columns.BeginUpdate();
                AssociatedObject.SortInfo.BeginUpdate();
            } else if(isGridLocked && !needLock) {
                AssociatedObject.SortInfo.EndUpdate();
                AssociatedObject.Columns.EndUpdate();
                isGridLocked = false;
            }
        }
    }
}
