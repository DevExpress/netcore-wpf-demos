using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Pdf;
using DevExpress.Xpf.PdfViewer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace DevExpress.DevAV.Common.View {
    public class WatermarkBehavior : Behavior<PdfViewerControl> {
        readonly static PdfStringFormat watermarkFormat;
        public static readonly DependencyProperty DocumentSourceProperty =
            DependencyProperty.Register("DocumentSource", typeof(Stream), typeof(WatermarkBehavior), new PropertyMetadata(null, (d, e) => ((WatermarkBehavior)d).UpdateDocumnt()));
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(WatermarkBehavior), new PropertyMetadata(string.Empty, (d, e) => ((WatermarkBehavior)d).UpdateDocumnt()));

        public Stream DocumentSource { get { return (Stream)GetValue(DocumentSourceProperty); } set { SetValue(DocumentSourceProperty, value); } }
        public string Text { get { return (string)GetValue(TextProperty); } set { SetValue(TextProperty, value); } }

        static WatermarkBehavior() {
            watermarkFormat = new PdfStringFormat();
            watermarkFormat.FormatFlags = PdfStringFormatFlags.NoWrap | PdfStringFormatFlags.NoClip;
            watermarkFormat.Alignment = PdfStringAlignment.Center;
            watermarkFormat.LineAlignment = PdfStringAlignment.Center;
        }

        protected override void OnAttached() {
            base.OnAttached();
            UpdateDocumnt();
        }
        void UpdateDocumnt() {
            if(AssociatedObject == null || string.IsNullOrEmpty(Text) || DocumentSource == null) {
                AssociatedObject.DocumentSource = null;
                return;
            }
            using(PdfDocumentProcessor processor = new PdfDocumentProcessor()) {
                processor.LoadDocument(DocumentSource);
                AddWatermark(processor, Text);
                var tmpFile = Path.GetTempFileName();
                processor.SaveDocument(tmpFile);
                AssociatedObject.OpenDocument(tmpFile);
            }
        }

        static void AddWatermark(PdfDocumentProcessor processor, string watermark) {
            var pages = processor.Document.Pages;
            for(int i = 0; i < pages.Count; i++) {
                using(var graphics = processor.CreateGraphics()) {
                    using(Font font = new Font("Segoe UI", 48, System.Drawing.FontStyle.Regular)) {
                        RectangleF pageLayout = new RectangleF(
                             -(float)pages[i].CropBox.Width * 0.35f,
                             (float)pages[i].CropBox.Height * 0.1f,
                             (float)pages[i].CropBox.Width * 1.25f,
                             (float)pages[i].CropBox.Height);
                        var angle = Math.Asin((double)pageLayout.Width / (double)pageLayout.Height) * 180.0 / Math.PI;
                        graphics.TranslateTransform(-pageLayout.X, -pageLayout.Y);
                        graphics.RotateTransform((float)angle);

                        using(SolidBrush textBrush = new SolidBrush(Color.FromArgb(100, Color.Red)))
                            graphics.DrawString(watermark, font, textBrush, new PointF(50, 50));
                    }
                    graphics.AddToPageForeground(pages[i]);
                }
            }
        }
    }
}
