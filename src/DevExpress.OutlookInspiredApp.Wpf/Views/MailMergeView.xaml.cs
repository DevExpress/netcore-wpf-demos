using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.RichEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DevExpress.DevAV.Views {
    public partial class MailMergeView : UserControl {
        public MailMergeView() {
            InitializeComponent();
        }
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            richEdit.MailMergeOptions = RichEditMailMergeOptions;
            if(RichEditBehavior != null)
                Interaction.GetBehaviors(richEdit).Add(RichEditBehavior);
        }

        public static readonly DependencyProperty RichEditDocumentSourceProperty =
            DependencyProperty.Register("RichEditDocumentSource", typeof(object), typeof(MailMergeView), new PropertyMetadata(null));
        public static readonly DependencyProperty AdditionalParametersFormProperty =
            DependencyProperty.Register("AdditionalParametersForm", typeof(FrameworkElement), typeof(MailMergeView), new PropertyMetadata(null));

        public object RichEditDocumentSource { get { return GetValue(RichEditDocumentSourceProperty); } set { SetValue(RichEditDocumentSourceProperty, value); } }
        public FrameworkElement AdditionalParametersForm { get { return (FrameworkElement)GetValue(AdditionalParametersFormProperty); } set { SetValue(AdditionalParametersFormProperty, value); } }

        public Behavior RichEditBehavior { get; set; }
        public DXRichEditMailMergeOptions RichEditMailMergeOptions { get; set; }
    }
}
