using System;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows;
using DevExpress.Xpf.Charts;

namespace DevExpress.DevAV.Views {
    public partial class DashboardView : UserControl {
        const int highTopSpacing = 10;
        const int lowTopSpacing = -15;
        const int highBottomSpacing = 5;
        const int lowBottomSpacing = 0;
        const int heightThreshold = 150;

        public DashboardView() {
            InitializeComponent();
        }
        void goodsSold_SizeChanged(object sender, SizeChangedEventArgs e) {
            Legend legend = ((ChartControl)sender).Legend;
            if(e.NewSize.Height < heightThreshold && ((int)legend.Margin.Top != lowTopSpacing || (int)legend.Margin.Bottom != lowBottomSpacing))
                legend.Margin = new Thickness { Top = lowTopSpacing, Bottom = lowBottomSpacing };
            if(e.NewSize.Height >= heightThreshold && ((int)legend.Margin.Top != highTopSpacing || (int)legend.Margin.Bottom != highBottomSpacing))
                legend.Margin = new Thickness { Top = highTopSpacing, Bottom = highBottomSpacing };
        }
    }
}
