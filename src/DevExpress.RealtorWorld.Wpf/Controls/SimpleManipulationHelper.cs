using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;

namespace DevExpress.RealtorWorld.Xpf.Controls {
    public interface ISimpleManupulationSupport {
        void ScrollBy(double x, double y, bool isMouseManipulation);
        void ScaleBy(double factor, bool isMouseManipulation);
        void FinishManipulation(bool isMouseManipulation);
        double DesiredDeseleration { get; }
    }
    public class SimpleManipulationHelper {
        #region Dependency Properties
        public static readonly DependencyProperty OverrideManipulationProperty;
        static SimpleManipulationHelper() {
            Type ownerType = typeof(SimpleManipulationHelper);
            OverrideManipulationProperty = DependencyProperty.RegisterAttached("OverrideManipulation", typeof(bool), ownerType, new PropertyMetadata(false));
        }
        #endregion
        public static bool GetOverrideManipulation(DependencyObject d) { return (bool)d.GetValue(OverrideManipulationProperty); }
        public static void SetOverrideManipulation(DependencyObject d, bool value) { d.SetValue(OverrideManipulationProperty, value); }

        Point lastPosition;
        FrameworkElement owner;
        bool manipulationInProgress;
        DependencyPropertyDescriptor isMouseManipulationEnabledDescriptor;
        bool mouseMoveHandled;
        bool doNotProcessMouse = false;

        public SimpleManipulationHelper(FrameworkElement owner) {
            this.owner = owner;
            PropertyDescriptor pd = TypeDescriptor.GetProperties(this.owner)["IsMouseManipulationEnabled"];
            this.isMouseManipulationEnabledDescriptor = pd == null ? null : DependencyPropertyDescriptor.FromProperty(pd);
            this.owner.Loaded += OnOwnerLoaded;
            this.owner.Unloaded += OnOwnerUnloaded;
            this.owner.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(OnOwnerManipulationDelta);
            this.owner.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(OnOwnerManipulationCompleted);
            this.owner.ManipulationInertiaStarting += OnOwnerManipulationInertiaStarting;
        }
        void OnOwnerLoaded(object sender, RoutedEventArgs e) {
            if(this.isMouseManipulationEnabledDescriptor != null)
                this.isMouseManipulationEnabledDescriptor.AddValueChanged(this.owner, OnOwnerIsMouseManipulationEnabledChanged);
            OnOwnerIsMouseManipulationEnabledChanged(this.owner, EventArgs.Empty);
        }
        void OnOwnerUnloaded(object sender, RoutedEventArgs e) {
            if(this.isMouseManipulationEnabledDescriptor != null)
                this.isMouseManipulationEnabledDescriptor.RemoveValueChanged(this.owner, OnOwnerIsMouseManipulationEnabledChanged);
            UnsubscribeFromMouseEvents();
        }
        void OnOwnerIsMouseManipulationEnabledChanged(object sender, EventArgs e) {
            bool isManipulationEnabled = this.isMouseManipulationEnabledDescriptor == null ? false : (bool)this.isMouseManipulationEnabledDescriptor.GetValue(this.owner);
            if(isManipulationEnabled)
                SubscribeToMouseEvents();
            else
                UnsubscribeFromMouseEvents();
        }
        void SubscribeToMouseEvents() {
            this.owner.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(OnOwnerMouseLeftButtonDown);
            this.owner.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(OnOwnerMouseLeftButtonUp);
            this.owner.PreviewMouseMove += new MouseEventHandler(OnOwnerMouseMove);
            this.owner.PreviewMouseWheel += new MouseWheelEventHandler(OnOwnerMouseWheel);
        }
        void UnsubscribeFromMouseEvents() {
            this.owner.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(OnOwnerMouseLeftButtonDown);
            this.owner.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(OnOwnerMouseLeftButtonUp);
            this.owner.PreviewMouseMove -= new MouseEventHandler(OnOwnerMouseMove);
            this.owner.PreviewMouseWheel -= new MouseWheelEventHandler(OnOwnerMouseWheel);
        }
        void OnOwnerManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e) {
            ISimpleManupulationSupport sms = this.owner as ISimpleManupulationSupport;
            if(sms != null)
                e.TranslationBehavior.DesiredDeceleration = sms.DesiredDeseleration;
            e.Handled = true;
        }
        void OnOwnerManipulationCompleted(object sender, ManipulationCompletedEventArgs e) {
            e.Handled = true;
            ISimpleManupulationSupport sms = this.owner as ISimpleManupulationSupport;
            if(sms != null)
                sms.FinishManipulation(false);
            if(e.TotalManipulation.Translation.X == 0.0 && e.TotalManipulation.Translation.Y == 0.0 && e.TotalManipulation.Scale.X == 1.0 && e.TotalManipulation.Scale.Y == 1.0)
                RaiseClick();
        }
        void OnOwnerManipulationDelta(object sender, ManipulationDeltaEventArgs e) {
            ISimpleManupulationSupport sms = this.owner as ISimpleManupulationSupport;
            if(sms != null) {
                double sx = 1.0 + (e.DeltaManipulation.Scale.X - 1.0) / 1.0;
                double sy = 1.0 + (e.DeltaManipulation.Scale.Y - 1.0) / 1.0;
                double prec = 0.0005;
                bool b1 = Math.Abs(sx - 1.0) <= prec;
                bool b2 = Math.Abs(sy - 1.0) <= prec;
                if(!b1 || !b2)
                    sms.ScaleBy(sx, false);
                else
                    sms.ScrollBy(-e.DeltaManipulation.Translation.X, -e.DeltaManipulation.Translation.Y, false);
            }
            e.Handled = true;
        }
        void RaiseClick() {
            HitTestResult result = VisualTreeHelper.HitTest(this.owner, Mouse.GetPosition(this.owner));
            if(result == null) return;
            UIElement element = GetUIVisualHit(result.VisualHit);
            if(RaiseButtonClick(element)) return;
            if(RaiseFlowDocumentClick(element)) return;
            if(element == null)
                element = this.owner;
            MouseButtonEventArgs down = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = Mouse.MouseDownEvent, Source = element };
            element.RaiseEvent(down);
            InputManager.Current.ProcessInput(down);
            MouseButtonEventArgs up = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = Mouse.MouseUpEvent, Source = element };
            element.RaiseEvent(up);
            InputManager.Current.ProcessInput(up);
        }
        bool RaiseButtonClick(UIElement element) {
            ButtonBase button = element == null ? null : LayoutHelper.FindParentObject<ButtonBase>(element);
            if(button == null) return false;
            RoutedEventArgs click = new RoutedEventArgs(ButtonBase.ClickEvent, button);
            button.RaiseEvent(click);
            ICommand command = button.Command;
            if(command != null && command.CanExecute(button.CommandParameter) && button.IsEnabled)
                command.Execute(button.CommandParameter);
            return true;
        }
        bool RaiseFlowDocumentClick(UIElement element) {
            if(element == null || element.GetType().FullName != "MS.Internal.Documents.FlowDocumentView") return false;
            ScrollViewer scrollViewer = LayoutHelper.FindParentObject<ScrollViewer>(element);
            RichTextBox rtb = LayoutHelper.FindParentObject<RichTextBox>(element);
            if(scrollViewer == null || rtb == null || rtb.Document == null) return false;
            Point position = Mouse.GetPosition(scrollViewer);
            foreach(System.Windows.Documents.Block block in rtb.Document.Blocks) {
                System.Windows.Documents.Paragraph paragraph = block as System.Windows.Documents.Paragraph;
                if(paragraph == null) continue;
                foreach(System.Windows.Documents.Inline inline in paragraph.Inlines) {
                    System.Windows.Documents.Hyperlink hyperlink = inline as System.Windows.Documents.Hyperlink;
                    if(hyperlink == null) continue;
                    Rect start = hyperlink.ContentStart.GetCharacterRect(System.Windows.Documents.LogicalDirection.Forward);
                    Rect end = hyperlink.ContentEnd.GetCharacterRect(System.Windows.Documents.LogicalDirection.Backward);
                    if(position.X < start.Left || position.X > end.Right) continue;
                    if(position.Y < start.Top || position.Y > end.Bottom) continue;
                    RaiseHyperlinkClick(hyperlink);
                    return true;
                }
            }
            return false;
        }
        void RaiseHyperlinkClick(System.Windows.Documents.Hyperlink hyperlink) {
            RoutedEventArgs click = new RoutedEventArgs(System.Windows.Documents.Hyperlink.ClickEvent, hyperlink);
            hyperlink.RaiseEvent(click);
            ICommand command = hyperlink.Command;
            if(command != null && command.CanExecute(hyperlink.CommandParameter) && hyperlink.IsEnabled)
                command.Execute(hyperlink.CommandParameter);
        }
        UIElement GetUIVisualHit(DependencyObject d) {
            if(d == null) return null;
            UIElement element = d as UIElement;
            if(element != null) return element;
            return LayoutHelper.FindParentObject<UIElement>(d);
        }
        void OnOwnerMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if(doNotProcessMouse) return;
            HitTestResult result = VisualTreeHelper.HitTest(this.owner, e.GetPosition(this.owner));
            UIElement element = result == null ? null : result.VisualHit as UIElement;
            DependencyObject dom = element == null ? null : LayoutHelper.FindLayoutOrVisualParentObject(element, d => GetOverrideManipulation(d));
            if(dom != null) return;
            if(!this.owner.CaptureMouse()) return;
            mouseMoveHandled = false;
            e.Handled = true;
            this.lastPosition = e.GetPosition(this.owner);
            this.manipulationInProgress = true;
        }
        void OnOwnerMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if(!this.manipulationInProgress) return;
            e.Handled = true;
            this.manipulationInProgress = false;
            this.owner.ReleaseMouseCapture();
            ISimpleManupulationSupport sms = this.owner as ISimpleManupulationSupport;
            if(sms != null)
                sms.FinishManipulation(true);
            if(!mouseMoveHandled) {
                doNotProcessMouse = true;
                RaiseClick();
                doNotProcessMouse = false;
            }
        }
        void OnOwnerMouseMove(object sender, MouseEventArgs e) {
            if(!this.manipulationInProgress) return;
            mouseMoveHandled = true;
            Point newPosition = e.GetPosition(this.owner);
            ISimpleManupulationSupport sms = this.owner as ISimpleManupulationSupport;
            if(sms != null)
                sms.ScrollBy(this.lastPosition.X - newPosition.X, this.lastPosition.Y - newPosition.Y, true);
            this.lastPosition = newPosition;
        }
        void OnOwnerMouseWheel(object sender, MouseWheelEventArgs e) {
            if((Keyboard.Modifiers & ModifierKeys.Control) == 0) {
                e.Handled = true;
                ISimpleManupulationSupport sms = this.owner as ISimpleManupulationSupport;
                if(sms != null) {
                    RenderScrollViewer rsv = this.owner as RenderScrollViewer;
                    if(rsv == null) return;
                    bool vScroll = rsv.ComputedVerticalScrollBarVisibility == Visibility.Visible;
                    bool hScroll = rsv.ComputedHorizontalScrollBarVisibility == Visibility.Visible;
                    double delta = -e.Delta * 1.0;
                    if(vScroll)
                        sms.ScrollBy(0, delta, true);
                    else if(hScroll)
                        sms.ScrollBy(delta, 0, true);
                }
            } else {
                e.Handled = true;
                ISimpleManupulationSupport sms = this.owner as ISimpleManupulationSupport;
                if(sms != null)
                    sms.ScaleBy((double)e.Delta, true);
            }
        }
    }
}
