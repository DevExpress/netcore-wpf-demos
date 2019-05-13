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
using DevExpress.XtraRichEdit;

namespace DevExpress.DevAV {
    public class RichEditControlMailMergeBehavior : Behavior<RichEditControl> {
        public object ActiveObject {
            get { return (object)GetValue(ActiveObjectProperty); }
            set { SetValue(ActiveObjectProperty, value); }
        }
        public static readonly DependencyProperty ActiveObjectProperty =
            DependencyProperty.Register("ActiveObject", typeof(object), typeof(RichEditControlMailMergeBehavior), new PropertyMetadata(null, (d, e) => ((RichEditControlMailMergeBehavior)d).UpdateActiveRecord()));

        public Stream DocumentTemplate {
            get { return (Stream)GetValue(DocumentTemplateProperty); }
            set { SetValue(DocumentTemplateProperty, value); }
        }
        public static readonly DependencyProperty DocumentTemplateProperty =
            DependencyProperty.Register("DocumentTemplate", typeof(Stream), typeof(RichEditControlMailMergeBehavior), new PropertyMetadata(null, (d, e) => ((RichEditControlMailMergeBehavior)d).UpdateDocumentTemplate()));

        public IEnumerable DataSource {
            get { return (IEnumerable)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(IEnumerable), typeof(RichEditControlMailMergeBehavior), new PropertyMetadata(null, (d, e) => ((RichEditControlMailMergeBehavior)d).UpdateDataSource()));


        public IEnumerable<string> RemoveFields {
            get { return (IEnumerable<string>)GetValue(RemoveFieldsProperty); }
            set { SetValue(RemoveFieldsProperty, value); }
        }
        public static readonly DependencyProperty RemoveFieldsProperty =
            DependencyProperty.Register("RemoveFields", typeof(IEnumerable<string>), typeof(RichEditControlMailMergeBehavior), new PropertyMetadata(null));

        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.ApplyTemplate();
            AssociatedObject.Options.MailMerge.ActiveRecord = -1;

            AssociatedObject.Options.MailMerge.ViewMergedData = true;
            AssociatedObject.CustomizeMergeFields += CustomizeMergeFields;
            AssociatedObject.ActiveRecordChanged += ActiveRecordChanged;
            UpdateDataSource();
        }

        void ActiveRecordChanged(object sender, EventArgs e) {
            //ActiveObject = AssociatedObject.Options.MailMerge.ActiveRecord >= 0 ? DataSource.Cast<object>().ElementAt(AssociatedObject.Options.MailMerge.ActiveRecord) : null;
        }

        protected override void OnDetaching() {
            base.OnDetaching();
            AssociatedObject.CustomizeMergeFields -= CustomizeMergeFields;
            AssociatedObject.ActiveRecordChanged -= ActiveRecordChanged;
        }

        void CustomizeMergeFields(object sender, XtraRichEdit.CustomizeMergeFieldsEventArgs e) {
            if(RemoveFields != null)
                e.MergeFieldsNames = e.MergeFieldsNames.Where(fn => !RemoveFields.Any(x => x == fn.Name)).ToArray();
        }

        void UpdateDataSource() {
            if (AssociatedObject != null) {
                AssociatedObject.ApplyTemplate();
                AssociatedObject.Options.MailMerge.DataSource = DataSource;
                AssociatedObject.Document.Fields.Update();
            }
            UpdateActiveRecord();
        }

        void UpdateDocumentTemplate() {
            if(AssociatedObject != null) {
                AssociatedObject.ApplyTemplate();
                int index = AssociatedObject.Options.MailMerge.ActiveRecord;
                AssociatedObject.LoadDocumentTemplate(DocumentTemplate, DocumentFormat.Rtf);
                AssociatedObject.Options.MailMerge.ActiveRecord = index;
                AssociatedObject.Document.Fields.Update();
            }
        }

        void UpdateActiveRecord() {
            if(AssociatedObject != null && DataSource != null) {
                var index = DataSource.Cast<object>().Select((x, i) => new { item = x, index = i }).FirstOrDefault(x => x.item == ActiveObject);
                AssociatedObject.Options.MailMerge.ActiveRecord = index != null ? index.index : -1;
            }
        }
    }
}