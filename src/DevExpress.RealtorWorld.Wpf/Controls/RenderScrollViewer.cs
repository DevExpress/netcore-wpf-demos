using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#if REALTORWORLDDEMO
using DevExpress.Xpf.Core;
#endif

#if REALTORWORLDDEMO
namespace DevExpress.RealtorWorld.Xpf.Controls {
#else
namespace DXperienceDemos.Internal.Helpers {
#endif
    [TemplatePart(Name = "ScrollContentControl", Type = typeof(RenderScrollContentControl))]
    public class RenderScrollViewer : ContentControl, ISimpleManupulationSupport {
        #region Dependency Properties
        public static readonly DependencyProperty DesiredDeselerationProperty;
        public static readonly DependencyProperty IsMouseManipulationEnabledProperty;
        public static readonly DependencyProperty ComputedLeftShadowVisibilityProperty;
        public static readonly DependencyProperty ComputedRightShadowVisibilityProperty;
        static RenderScrollViewer() {
            Type ownerType = typeof(RenderScrollViewer);
            DesiredDeselerationProperty = DependencyProperty.Register("DesiredDeseleration", typeof(double), ownerType, new PropertyMetadata(0.001));
            IsMouseManipulationEnabledProperty = DependencyProperty.Register("IsMouseManipulationEnabled", typeof(bool), ownerType, new PropertyMetadata(false));
            ComputedRightShadowVisibilityProperty = DependencyProperty.Register("ComputedRightShadowVisibility", typeof(Visibility), ownerType, new PropertyMetadata(Visibility.Collapsed));
            ComputedLeftShadowVisibilityProperty = DependencyProperty.Register("ComputedLeftShadowVisibility", typeof(Visibility), ownerType, new PropertyMetadata(Visibility.Collapsed));
        }
        #endregion
        RenderScrollContentControl scrollContentControl;
        SimpleManipulationHelper smh;

        public RenderScrollViewer() {
            this.SetDefaultStyleKey(typeof(RenderScrollViewer));
#if REALTORWORLDDEMO
            FocusHelper2.SetFocusable(this, false);
#else
            Focusable = false;
#endif
            this.smh = new SimpleManipulationHelper(this);
        }
        public double DesiredDeseleration { get { return (double)GetValue(DesiredDeselerationProperty); } set { SetValue(DesiredDeselerationProperty, value); } }
        public bool IsMouseManipulationEnabled { get { return (bool)GetValue(IsMouseManipulationEnabledProperty); } set { SetValue(IsMouseManipulationEnabledProperty, value); } }
        public Visibility ComputedLeftShadowVisibility { get { return (Visibility)GetValue(ComputedLeftShadowVisibilityProperty); } set { SetValue(ComputedLeftShadowVisibilityProperty, value); } }
        public Visibility ComputedRightShadowVisibility { get { return (Visibility)GetValue(ComputedRightShadowVisibilityProperty); } set { SetValue(ComputedRightShadowVisibilityProperty, value); } }
        public double HorizontalOffset { get { return this.scrollContentControl == null ? 0.0 : this.scrollContentControl.HorizontalOffset; } }
        public double VerticalOffset { get { return this.scrollContentControl == null ? 0.0 : this.scrollContentControl.VerticalOffset; } }
        public double HorizontalRelative { get { return this.scrollContentControl == null ? 0.0 : this.scrollContentControl.HorizontalValue / this.scrollContentControl.HorizontalMaximum; } }
        public double VerticalRelative { get { return this.scrollContentControl == null ? 0.0 : this.scrollContentControl.VerticalValue / this.scrollContentControl.VerticalMaximum; } }
        public double ViewportWidth { get { return this.scrollContentControl == null ? 0.0 : this.scrollContentControl.ActualWidth; } }
        public double ViewportHeight { get { return this.scrollContentControl == null ? 0.0 : this.scrollContentControl.ActualHeight; } }
        public Visibility ComputedHorizontalScrollBarVisibility { get { return this.scrollContentControl == null ? Visibility.Collapsed : this.scrollContentControl.ComputedHorizontalScrollBarVisibility; } }
        public Visibility ComputedVerticalScrollBarVisibility { get { return this.scrollContentControl == null ? Visibility.Collapsed : this.scrollContentControl.ComputedVerticalScrollBarVisibility; } }
        public event EventHandler ScrollChanged;
        public event EventHandler ViewportSizeChanged;
        public void ScrollToHorizontalOffset(double x) {
            if(this.scrollContentControl != null)
                this.scrollContentControl.ScrollToHorizontalOffset(x);
        }
        public void ScrollToVerticalOffset(double y) {
            if(this.scrollContentControl != null)
                this.scrollContentControl.ScrollToVerticalOffset(y);
        }
        public void ScrollToHorizontalRelative(double x) {
            if(this.scrollContentControl != null)
                this.scrollContentControl.ScrollToHorizontalRelative(x);
        }
        public void ScrollToVerticalRelative(double y) {
            if(this.scrollContentControl != null)
                this.scrollContentControl.ScrollToVerticalRelative(y);
        }
        void OnScrollContentControlSizeChanged(object sender, SizeChangedEventArgs e) {
            if(ViewportSizeChanged != null)
                ViewportSizeChanged(this, EventArgs.Empty);
        }
        void OnScrollContentControlScrollChanged(object sender, EventArgs e) {
            UpdateShadowsVisibility();
            if(ScrollChanged != null)
                ScrollChanged(this, EventArgs.Empty);
        }
        void OnScrollContentControlHorizontalScrollBarVisibilityChanged(object sender, EventArgs e) {
            UpdateShadowsVisibility();
        }
        void OnScrollContentControlVerticalScrollBarVisibilityChanged(object sender, EventArgs e) {
            UpdateShadowsVisibility();
        }
        void UpdateShadowsVisibility() {
            if(this.scrollContentControl.ComputedHorizontalScrollBarVisibility == Visibility.Visible) {
                ComputedLeftShadowVisibility = HorizontalRelative > 0.0 ? Visibility.Visible : Visibility.Collapsed;
                ComputedRightShadowVisibility = HorizontalRelative < 1.0 ? Visibility.Visible : Visibility.Collapsed;
            } else {
                ComputedLeftShadowVisibility = Visibility.Collapsed;
                ComputedRightShadowVisibility = Visibility.Collapsed;
            }
        }
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            this.scrollContentControl = (RenderScrollContentControl)GetTemplateChild("ScrollContentControl");
            if(this.scrollContentControl != null) {
                this.scrollContentControl.SizeChanged += OnScrollContentControlSizeChanged;
                this.scrollContentControl.ScrollChanged += OnScrollContentControlScrollChanged;
                this.scrollContentControl.ComputedHorizontalScrollBarVisibilityChanged += OnScrollContentControlHorizontalScrollBarVisibilityChanged;
                this.scrollContentControl.ComputedVerticalScrollBarVisibilityChanged += OnScrollContentControlVerticalScrollBarVisibilityChanged;
            }
        }
        #region ISimpleManupulationSupport
        public virtual void ScrollBy(double x, double y, bool isMouseManipulation) {
            ScrollToHorizontalOffset(HorizontalOffset + x);
            ScrollToVerticalOffset(VerticalOffset + y);
        }
        public virtual void ScaleBy(double factor, bool isMouseManipulation) { }
        public virtual void FinishManipulation(bool isMouseManipulation) { }
        #endregion
    }
}
