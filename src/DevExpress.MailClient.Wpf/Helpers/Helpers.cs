using DevExpress.DevAV;
using DevExpress.Images;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraRichEdit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#if DXCORE3
using Microsoft.EntityFrameworkCore;
using DevExpress.Internal;
#else
using System.Data.Entity;
#endif

namespace DevExpress.MailClient.Helpers {
    public static class DataHelper {
        static List<Employee> employees = null;
        static RichEditDocumentServer server = new RichEditDocumentServer();
        static ImageSource unknownUserPicture;

        internal static List<Employee> Employees {
            get {
                if(employees == null) {
#if DXCORE3
                    SetFilePath();
                    var devAvDb = new DevAVDb(string.Format("Data Source={0}", filePath));
                    devAvDb.Pictures.Load();
#else
                    var devAvDb = new DevAVDb();
#endif
                    devAvDb.Employees.Load();
                    employees = devAvDb.Employees.Local.ToList();
                }
                return employees;
            }
        }
#if DXCORE3
        static string filePath;
        static void SetFilePath() {
            if(filePath == null)
                filePath = DataDirectoryHelper.GetFile("devav.sqlite3", DataDirectoryHelper.DataFolderName);
            try {
                var attributes = File.GetAttributes(filePath);
                if(attributes.HasFlag(FileAttributes.ReadOnly)) {
                    File.SetAttributes(filePath, attributes & ~FileAttributes.ReadOnly);
                }
            } catch { }
        }
#endif
        internal static ImageSource UnknownUserPicture {
            get {
                if(unknownUserPicture == null) {
                    unknownUserPicture = new BitmapImage(FilePathHelper.GetAppImageUri("Contacts/Unknown-user", UriKind.RelativeOrAbsolute));
                    return unknownUserPicture;
                }
                return unknownUserPicture;
            }
        }
        public static string GetNameByEmail(string mail, bool isDesignMode = false) {
            if(string.IsNullOrEmpty(mail) || isDesignMode)
                return string.Empty;
            var employee = Employees.FirstOrDefault(p => p.Email == mail);
            return employee == null ? string.Empty : employee.FullName;
        }
        public static ImageSource GetPictureByEmail(string mail, bool isDesignMode = false) {
            if(string.IsNullOrEmpty(mail) || isDesignMode)
                return UnknownUserPicture;
            var employee = Employees.FirstOrDefault(p => p.Email == mail);
            if(employee != null && employee.Picture != null && employee.Picture.Data != null)
                return ImageHelper.CreateImageFromStream(new MemoryStream(employee.Picture.Data as byte[]));
            return UnknownUserPicture;
        }
        public static string GetPlainTextFromMHT(string mhtText) {
            server.MhtText = mhtText;
            return server.Text.TrimStart();
        }
        public static string GetMHTTextFromHTML(string htmlText) {
            server.HtmlText = htmlText;
            return server.MhtText;
        }
        public static bool HasImages(string mhtText) {
            server.MhtText = mhtText;
            return server.Document.Images.Any();
        }
    }
    public class EmailValidationHelper {
        static readonly Regex regex = new Regex(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
        static readonly string[] splitters = new string[] { ",", ";" };

        public static bool IsEmailValid(string emailAddress) {
            return regex.Match(emailAddress).Length > 0;
        }
        public static IEnumerable<string> ExtractEmails(string emailsString) {
            string[] possibleEmails = emailsString.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
            return possibleEmails.Where(x => !string.IsNullOrWhiteSpace(x)).Select(o => o.Trim());
        }
        public static string NormalizeEmailsString(string emailsString) {
            return string.IsNullOrWhiteSpace(emailsString) ? string.Empty : string.Join(",", ExtractEmails(emailsString));
        }
    }
    public class FilePathHelper {
        public static string GetFullPath(string name) {
            var type = Type.GetType("DevExpress.DemoData.Helpers.DataFilesHelper, " + AssemblyInfo.SRAssemblyDemoDataCore + ", Version=" + AssemblyInfo.Version + ", Culture=neutral, PublicKeyToken=" + AssemblyInfo.PublicKeyToken);
            var method = type.GetMethod("FindFile", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            var propValue = "Data";
            return (string)method.Invoke(null, new object[] { name, propValue });
        }
        public static Uri GetDXImageUri(string path) {
            return AssemblyHelper.GetResourceUri(typeof(DXImages).Assembly, string.Format("Office2013/{0}.png", path));
        }
        public static Uri GetAppImageUri(string path, UriKind uriKind = UriKind.Relative) {
            return new Uri(string.Format("/DevExpress.MailClient.Xpf;component/Images/{0}.png", path), uriKind);
        }
    }
}
