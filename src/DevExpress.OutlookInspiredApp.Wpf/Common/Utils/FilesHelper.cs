using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DevExpress.DevAV.Common.Utils {
    public static class FilesHelper {
        #region CommonWinApi
        [Flags]
        public enum SHGFI : int {
            Icon = 0x000000100,
            DisplayName = 0x000000200,
            TypeName = 0x000000400,
            Attributes = 0x000000800,
            IconLocation = 0x000001000,
            ExeType = 0x000002000,
            SysIconIndex = 0x000004000,
            LinkOverlay = 0x000008000,
            Selected = 0x000010000,
            Attr_Specified = 0x000020000,
            LargeIcon = 0x000000000,
            SmallIcon = 0x000000001,
            OpenIcon = 0x000000002,
            ShellIconSize = 0x000000004,
            PIDL = 0x000000008,
            UseFileAttributes = 0x000000010,
            AddOverlays = 0x000000020,
            OverlayIndex = 0x000000040,
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SHFILEINFO {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        class Shell32 {
            [DllImport("shell32.dll")]
            public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
            [DllImport("User32.dll")]
            public static extern int DestroyIcon(IntPtr hIcon);
        }
        #endregion

        public static long MaxAttachedFileSize = 6;
        public static AttachedFileInfo GetAttachedFileInfo(string name, string directoryName = "", long id = -1) {
            return new AttachedFileInfo() {
                Name = name,
                DisplayName = Path.GetFileNameWithoutExtension(name),
                Extension = Path.GetExtension(name),
                Icon = IconToImageSourceConverter(IconFromExtension(Path.GetExtension(name))),
                FullPath = Path.Combine(directoryName, name),
                Id = id
            };
        }
        public static string OpenFileFromDb(string name, byte[] content) {
            string tempFileName = Path.GetTempFileName();
            string newFileName = Path.Combine(Path.GetDirectoryName(tempFileName), Path.GetFileNameWithoutExtension(name)
                + "_" + Path.GetFileNameWithoutExtension(tempFileName) + Path.GetExtension(name));
            File.Delete(tempFileName);
            File.WriteAllBytes(newFileName, content);
            File.SetAttributes(newFileName, FileAttributes.ReadOnly);
            Process.Start(newFileName);
            return newFileName;
        }
        public static void OpenFileFromDisc(string fullPath) {
            Process.Start(fullPath);
        }
        public static void DeleteTempFiles(ref List<string> deletedFilesPath) {
            if(deletedFilesPath == null) return;
            foreach(string path in deletedFilesPath) {
                try {
                    File.SetAttributes(path, FileAttributes.Normal);
                    File.Delete(path);
                } catch(Exception) { };
            }
            deletedFilesPath.Clear();
        }
        static Icon IconFromExtension(string extension) {
            SHFILEINFO shFileInfo = new SHFILEINFO();
            Shell32.SHGetFileInfo(extension, 0x80, ref shFileInfo, (uint)Marshal.SizeOf(shFileInfo), (uint)(SHGFI.Icon | SHGFI.LargeIcon | SHGFI.UseFileAttributes));
            Icon icon = (Icon)Icon.FromHandle(shFileInfo.hIcon).Clone();
            Shell32.DestroyIcon(shFileInfo.hIcon);
            return icon;
        }
        static BitmapSource IconToImageSourceConverter(Icon icon) {
            return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
    }
    public class AttachedFileInfo {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public ImageSource Icon { get; set; }
        public string FullPath { get; set; }
        public long Id { get; set; }
    }

}
