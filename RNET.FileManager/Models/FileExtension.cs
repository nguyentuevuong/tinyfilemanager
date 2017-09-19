using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using m = RNET.FileManager.Models;

namespace RNET.FileManager
{
    public class FileExtension
    {
        private FileExtension() { }

        public Image ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            if (maxWidth == 0 && maxHeight == 0)
                return image;

            if (maxWidth == 0)
                maxWidth = maxHeight / 3 * 4;

            if (maxHeight == 0)
                maxHeight = maxWidth / 4 * 3;

            int sourceWidth = image.Width;
            int sourceHeight = image.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)maxWidth / (float)sourceWidth);
            nPercentH = ((float)maxHeight / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((maxWidth -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((maxHeight -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Image bmPhoto = new Bitmap(maxWidth, maxHeight, PixelFormat.Format32bppArgb);

            using (Graphics grPhoto = Graphics.FromImage(bmPhoto))
            {
                grPhoto.Clear(Color.Transparent);
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

                grPhoto.DrawImage(image,
                    new Rectangle(destX, destY, destWidth, destHeight),
                    new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                    GraphicsUnit.Pixel);
            }
            return bmPhoto;
        }

        public Image ResizeImageFixRatio(Image image, int maxWidth, int maxHeight)
        {
            var ratio = maxWidth != 0 && maxHeight == 0 ? (double)maxWidth / image.Width : maxWidth == 0 && maxHeight != 0 ? (double)maxHeight / image.Height : 0;

            if (ratio == 0)
            {
                var newWidth = maxWidth;
                var newHeight = maxHeight;
                var newImage = new Bitmap(newWidth, newHeight);

                using (var graphic = Graphics.FromImage(newImage))
                    graphic.DrawImage(image, 0, 0, newWidth, newHeight);

                return newImage;
            }

            if (ratio < 1)
            {
                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);
                var newImage = new Bitmap(newWidth, newHeight);

                using (var graphic = Graphics.FromImage(newImage))
                    graphic.DrawImage(image, 0, 0, newWidth, newHeight);

                return newImage;
            }

            return image;
        }

        public ImageCodecInfo this[string mimeType]
        {
            get
            {
                return ImageCodecInfo.GetImageEncoders().FirstOrDefault(m => m.MimeType == mimeType);
            }
        }

        public string GetUrl(string fileUrl)
        {
            var fileName = fileUrl.ToLower().Replace(m.Configuration.Instance.UploadURL.ToLower(), "").Replace("/", "\\");

            var fileInfo = File.Exists(fileName);

            return fileInfo ? fileUrl : "";
        }

        public List<TreeNode> LoadFullHierarchy(string root = "")
        {
            if (string.IsNullOrEmpty(root))
                root = m.Configuration.Instance.UploadFullPath;

            var list = new List<TreeNode>();
            var ds = Directory.GetDirectories(root);
            foreach (var d in ds)
            {
                var node = new TreeNode();

                node.label = Path.GetFileName(d).ToLower();
                node.path = String.Format(@"{0}/{1}",
                    root.Replace("\\", "/"),
                    Path.GetFileName(d)).Replace('\\', '/').ToLower();

                node.children = LoadFullHierarchy(node.path);
                node.path = node.path.Replace(m.Configuration.Instance.UploadFullPath.ToLower().Replace("\\", "/"), "");

                list.Add(node);
            }


            return list;
        }

        private static FileExtension _Instance = null;

        public static FileExtension Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = Nested.Initialize;

                return _Instance;
            }
        }

        private class Nested
        {
            internal static readonly FileExtension Initialize = new FileExtension();
        }
    }

    public static class TextUtils
    {
        public static string StripDiacritics(this string accented)
        {
            if (String.IsNullOrEmpty(accented))
                return String.Empty;

            var regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string strFormD = accented.Normalize(NormalizationForm.FormD);
            return regex.Replace(strFormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D').Replace(' ', '-');
        }
    }
}