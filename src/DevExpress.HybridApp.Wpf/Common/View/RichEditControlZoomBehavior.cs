using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.RichEdit;
using System.Collections;
using System.IO;
using System.Windows.Input;
using DevExpress.Mvvm;

namespace DevExpress.DevAV {
    public class RichEditControlZoomBehavior : Behavior<RichEditControl> {
        static float minZoomFactor = 0.3f;
        static float maxZoomFactor = 1.7f;
        static float stepZoomFactor = 0.1f;

        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }

        public RichEditControlZoomBehavior() {
            ZoomInCommand = new DelegateCommand(
                () => AssociatedObject.ActiveView.ZoomFactor += stepZoomFactor, 
                () => AssociatedObject != null && AssociatedObject.ActiveView != null && AssociatedObject.ActiveView.ZoomFactor + stepZoomFactor < maxZoomFactor);
            ZoomOutCommand = new DelegateCommand(
                () => AssociatedObject.ActiveView.ZoomFactor -= stepZoomFactor, 
                () => AssociatedObject != null && AssociatedObject.ActiveView != null && AssociatedObject.ActiveView.ZoomFactor - stepZoomFactor > minZoomFactor);
        }

        //protected override void OnAttached() {
        //    base.OnAttached();
        //    AssociatedObject.ZoomChanged += ZoomChanged;
        //}

        //protected override void OnDetaching() {
        //    base.OnDetaching();
        //    AssociatedObject.ZoomChanged -= ZoomChanged;
        //}
    }
}