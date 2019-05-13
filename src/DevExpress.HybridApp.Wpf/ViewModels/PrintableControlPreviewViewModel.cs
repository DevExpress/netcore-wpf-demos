using System;
using System.ComponentModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Printing;

namespace DevExpress.DevAV.ViewModels {
    public class PrintableControlPreviewViewModel : IDocumentContent {
        public static PrintableControlPreviewViewModel Create(PrintableControlLink link) {
            return ViewModelSource.Create(() => new PrintableControlPreviewViewModel(link));
        }

        protected PrintableControlPreviewViewModel(PrintableControlLink link) {
            Link = link;
        }       
        public virtual PrintableControlLink Link { get; protected set; }

        public void Close() {
            if(DocumentOwner != null)
                DocumentOwner.Close(this);
        }
        protected IDocumentOwner DocumentOwner { get; private set; }
        #region IDocumentContent
        void IDocumentContent.OnClose(CancelEventArgs e) { }
        void IDocumentContent.OnDestroy() { }
        IDocumentOwner IDocumentContent.DocumentOwner {
            get { return DocumentOwner; }
            set { DocumentOwner = value; }
        }
        object IDocumentContent.Title { get { return null; } }
        #endregion
    }
}