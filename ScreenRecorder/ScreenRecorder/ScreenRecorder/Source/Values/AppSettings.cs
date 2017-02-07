using System;
using System.Drawing.Imaging;

namespace ScreenRecorder.Source.Values
{
    public static class AppSettings
    {
        public static string SaveFolderPath { get { return "D:/ScreenRecords"; } }

        public static ImageFormat ImgFormat
        {
            get
            {
                return ImageFormat.Jpeg;
            }
        }

        public static string SaveFileName
        {
            get
            {
                string s = string.Empty;
                s = string.Concat(
                    AppSettings.SaveFolderPath, AppValues.SaveFolderChar, AppValues.SaveFilePrefix, AppValues.LowerLine,
                    DateTime.Now.ToString(AppValues.SaveTimeFormat), FileExtension);
                return s;
            }
        }

        public static string FileExtension
        {
            get
            {
                string s = ".";

                if (ImgFormat == ImageFormat.Bmp) { s += "bmp"; return s; }
                if (ImgFormat == ImageFormat.Emf) { s += "emf"; return s; }
                if (ImgFormat == ImageFormat.Exif) { s += "exif"; return s; }
                if (ImgFormat == ImageFormat.Gif) { s += "gif"; return s; }
                if (ImgFormat == ImageFormat.Icon) { s += "ico"; return s; }
                if (ImgFormat == ImageFormat.Jpeg) { s += "jpeg"; return s; }
                if (ImgFormat == ImageFormat.MemoryBmp) { s += "bmp"; return s; }
                if (ImgFormat == ImageFormat.Png) { s += "png"; return s; }
                if (ImgFormat == ImageFormat.Tiff) { s += "tiff"; return s; }
                if (ImgFormat == ImageFormat.Wmf) { s += "wmf"; return s; }

                s += "jpeg";
                return s;
            }
        }

        public static int ImageDelayTime
        {
            get { return 1000 / 36; }
        }

        public static bool IsHQ { get { return true; } }
    }
}
