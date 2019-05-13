using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Printing;

namespace DevExpress.DevAV.Common.View {
    public interface IPrintableControlPreviewService {
        PrintableControlLink GetLink();
    }

    public class PrintableControlPreviewService : ServiceBase, IPrintableControlPreviewService {

        public bool IsLandscape { get; set; }

        public PrintableControlLink GetLink() {
            PrintableControlLink link = new PrintableControlLink(AssociatedObject as IPrintableControl);
            link.Landscape = IsLandscape;            
            link.CreateDocument(true);
            return link;
        }
    }
}
