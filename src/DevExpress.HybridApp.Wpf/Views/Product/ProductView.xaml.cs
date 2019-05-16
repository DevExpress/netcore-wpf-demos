using System;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;

namespace DevExpress.DevAV.Views {
    public partial class ProductView : UserControl {
        public ProductView() {
            InitializeComponent();
        }
        void PdfViewerControl_ManipulationBoundaryFeedback(object sender, System.Windows.Input.ManipulationBoundaryFeedbackEventArgs e) {
            e.Handled = true;
        }
    }
}
