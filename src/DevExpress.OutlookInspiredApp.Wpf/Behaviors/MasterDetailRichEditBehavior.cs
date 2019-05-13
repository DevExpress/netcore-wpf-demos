using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.DevAV.ViewModels;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.RichEdit;
using DevExpress.XtraRichEdit;

namespace DevExpress.DevAV {
    public class MasterDetailRichEditBehavior : Behavior<RichEditControl> {
        public static readonly DependencyProperty OrderProperty =
            DependencyProperty.Register("Order", typeof(Order), typeof(MasterDetailRichEditBehavior), 
                new PropertyMetadata(null, (d, e) => ((MasterDetailRichEditBehavior)d).OnOrderChanged()));

        public Order Order { get { return (Order)GetValue(OrderProperty); } set { SetValue(OrderProperty, value); } }

        IRichEditDocumentServer masterTemplate;
        IRichEditDocumentServer detailTemplate;
        RichEditControl richEdit;

        protected override void OnAttached() {
            base.OnAttached();
            richEdit = AssociatedObject;
            CreateTemplates();
            SubscribeEvents();
            OnOrderChanged();
        }
        protected override void OnDetaching() {
            base.OnDetaching();
            UnsubscribeEvents();
        }
        void OnCalculateDocumentVariable(object sender, CalculateDocumentVariableEventArgs e) {
            if(Order == null || e.VariableName != "Products")
                return;
            using(var server = new RichEditDocumentServer()) {
                using(var stream = MailMergeTemplatesHelper.GetTemplateStream("Sales Order Follow-Up Detail.rtf")) {
                    server.LoadDocument(stream, DocumentFormat.Rtf);
                    var options = server.CreateMailMergeOptions();
                    options.DataSource = Order.OrderItems;
                    server.MailMerge(options, detailTemplate.Document);
                }
            }
            e.Value = detailTemplate;
            e.KeepLastParagraph = true;
            e.Handled = true;
        }
        void OnOrderChanged() {
            if(Order == null || richEdit == null)
                return;
            using(var stream = MailMergeTemplatesHelper.GetTemplateStream("Sales Order Follow-Up.rtf")) {
                masterTemplate.LoadDocument(stream, DocumentFormat.Rtf);
                var options = masterTemplate.CreateMailMergeOptions();
                options.DataSource = new List<Order>() { Order };
                masterTemplate.MailMerge(options, richEdit.Document);
            }
        }
        void SubscribeEvents() {
            richEdit.CalculateDocumentVariable += OnCalculateDocumentVariable;
        }
        void UnsubscribeEvents() {
            richEdit.CalculateDocumentVariable -= OnCalculateDocumentVariable;
        }
        void CreateTemplates() {
            masterTemplate = richEdit.CreateDocumentServer();
            detailTemplate = richEdit.CreateDocumentServer();
        }
    }
}
