using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using m = RNET.FileManager.Models;
using d = System.Drawing;
using i = System.Drawing.Imaging;
using System.IO;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Web.Security;

namespace RNET.FileManager
{
    public partial class Default : Page
    {
        private static HttpContext context
        {
            get
            {
                return HttpContext.Current;
            }
        }

        private static bool isLogin
        {
            get
            {
                return context.Session["userInfo"] != null;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(m.Configuration.Instance.ThumbFullPath))
                Directory.CreateDirectory(m.Configuration.Instance.ThumbFullPath);

            if (!Directory.Exists(m.Configuration.Instance.UploadFullPath))
                Directory.CreateDirectory(m.Configuration.Instance.UploadFullPath);

            GetConfigs();

            if (context.Request.RequestType == "POST")
            {
                var fileUpload = context.Request.Files["file"];
                var folderName = String.Format("{0}", context.Request.Form["folder"]).Trim('/').Replace('/', '\\');

                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    var fileName = Regex.Replace(fileUpload.FileName.ToLower(), "\\s+", " ").Replace(" ", "-").Replace("\\\\", "\\").StripDiacritics();
                    var thumbFileName = String.Format("{0}\\{1}\\{2}", m.Configuration.Instance.ThumbFullPath.TrimEnd('\\'), folderName, fileName);
                    var targetFileName = String.Format("{0}\\{1}\\{2}", m.Configuration.Instance.UploadFullPath.TrimEnd('\\'), folderName, fileName);


                    if (FileTypes.Instance[targetFileName] != FileType.PICTURE)
                        fileUpload.SaveAs(targetFileName);
                    else
                    {
                        Image bmpPostedImage = new Bitmap(fileUpload.InputStream);

                        // images
                        Image objImage = FileExtension.Instance.ResizeImageFixRatio(bmpPostedImage, m.Configuration.Instance.ImageResizeWidth, m.Configuration.Instance.ImageResizeHeight);
                        if (objImage.RawFormat == d.Imaging.ImageFormat.Jpeg)
                        {
                            // Encoder parameter for image quality 
                            EncoderParameter qualityParam =
                                new EncoderParameter(i.Encoder.Quality, 100);
                            // Jpeg image codec 
                            ImageCodecInfo jpegCodec = FileExtension.Instance["image/jpeg"];

                            EncoderParameters encoderParams = new EncoderParameters(1);
                            encoderParams.Param[0] = qualityParam;
                            objImage.Save(targetFileName, d.Imaging.ImageFormat.Jpeg);
                        }
                        else
                        {
                            objImage.Save(targetFileName, objImage.RawFormat);
                        }

                        // thumbs
                        objImage = FileExtension.Instance.ResizeImage(bmpPostedImage, 185, 125);

                        objImage.Save(thumbFileName, objImage.RawFormat);
                    }
                }
            }
        }

        [WebMethod]
        public static object GetConfigs()
        {
            return m.Configuration.Instance.ToJson();
        }

        [WebMethod]
        public static object GetTree()
        {
            if (!isLogin)
                return new List<TreeNode>
                {
                    new TreeNode { label = "root", path = "/"}
                };


            return new List<TreeNode> { new TreeNode
                    {
                        label = "root",
                        path = "/",
                        children = FileExtension.Instance.LoadFullHierarchy()
                    }
                };
        }

        [WebMethod]
        public static object LogIn(string userName, string passWord)
        {
            if (userName == m.Configuration.Instance.UserInfo[0] && passWord == m.Configuration.Instance.UserInfo[1])
            {
                context.Session["userInfo"] = userName;
                return true;
            }
            else
                return false;
        }

        [WebMethod]
        public static object GetFiles(string filter, string folderPath)
        {
            folderPath = folderPath.Replace("/", "\\").Trim('\\');

            if (!Directory.Exists(String.Format(@"{0}\{1}", m.Configuration.Instance.UploadFullPath, folderPath)))
                folderPath = "";

            filter = filter ?? "*.*";
            if (String.IsNullOrEmpty(filter))
                filter = "*.*";

            if (!filter.Contains("."))
                filter = String.Format("*{0}*.*", filter);

            if (!filter.StartsWith("*"))
                filter = String.Format("*{0}", filter);

            if (!filter.EndsWith("*"))
                filter = String.Format("{0}*", filter);

            var objs = new FileManageModel
            {
                path = String.Format("/{0}", folderPath.Replace("\\", "/"))
            };

            if (!isLogin)
            {
                objs.items = new List<object>();
                return objs;
            }

            try
            {
                foreach (var fileName in Directory.GetDirectories(String.Format(@"{0}\{1}", m.Configuration.Instance.UploadFullPath, folderPath), filter))
                {
                    try
                    {
                        FileItem fileItem = new FileItem();

                        fileItem.FileType = FileType.FOLDER;
                        fileItem.Name = Path.GetFileName(fileName).ToLower();
                        fileItem.ThumbImage = "content/img/ico/folder.png";
                        fileItem.Path = String.Format(@"/{0}/{1}/{2}", m.Configuration.Instance.UploadPath.Replace("\\", "/"), folderPath.Replace("\\", "/"), Path.GetFileName(fileName)).Replace('\\', '/').ToLower();
                        fileItem.Path = Regex.Replace(fileItem.Path, "/{2,}", "/");
                        fileItem.Url = String.Format(@"/{0}/{1}", folderPath.Replace("\\", "/").TrimStart('/'), fileItem.Name.Replace('\\', '/').TrimStart('/')).ToLower();
                        fileItem.Url = Regex.Replace(fileItem.Url, "/{2,}", "/").Replace(":/", "://");

                        objs.items.Add(fileItem.ToJson());
                    }
                    catch
                    {
                        continue;
                    }
                }

                foreach (var fileName in Directory.GetFiles(String.Format(@"{0}\{1}", m.Configuration.Instance.UploadFullPath, folderPath), filter))
                {
                    try
                    {
                        FileItem fileItem = new FileItem();

                        fileItem.Name = Path.GetFileName(fileName);
                        fileItem.FileType = FileTypes.Instance[Path.GetFileName(fileName)];

                        fileItem.Path = String.Format(@"{0}/{1}", folderPath.Replace("\\", "/"), Path.GetFileName(fileName)).Replace('\\', '/').ToLower();
                        fileItem.Path = Regex.Replace(fileItem.Path, "/{2,}", "/");
                        fileItem.Url = String.Format(@"{0}/{1}/{2}", m.Configuration.Instance.UploadURL, folderPath.Replace("\\", "/"), fileItem.Name);
                        fileItem.Url = Regex.Replace(fileItem.Url, "/{2,}", "/").Replace(":/", "://");

                        if (fileItem.FileType != FileType.PICTURE)
                            fileItem.ThumbImage = String.Format(@"content/img/ico/{0}.png", Path.GetExtension(fileName).TrimStart('.').ToLower());
                        else
                            fileItem.ThumbImage = String.Format(@"{0}/{1}", m.Configuration.Instance.ThumbURL, fileItem.Path.Replace('\\', '/').TrimStart('/'));

                        objs.items.Add(fileItem.ToJson());
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            catch
            {
            }
            return objs;
        }

        [WebMethod]
        public static bool NewFolder(string folderName, string folderPath)
        {
            if (!isLogin)
                return false;

            try
            {
                folderName = folderName.ToLower().Replace(" ", "-").StripDiacritics();

                folderPath = folderPath.Replace('/', '\\').Trim('\\');

                Directory.CreateDirectory(String.Format(@"{0}\{1}\{2}", m.Configuration.Instance.ThumbFullPath, folderPath, folderName));
                Directory.CreateDirectory(String.Format(@"{0}\{1}\{2}", m.Configuration.Instance.UploadFullPath, folderPath, folderName));
                return true;
            }
            catch
            {
                return false;
            }
        }

        [WebMethod]
        public static bool Delete(string fileName, string folderPath)
        {
            if (!isLogin)
                return false;

            try
            {
                fileName = Regex.Replace(fileName.ToLower(), "\\s+", " ").Replace(" ", "-").Replace("\\\\", "\\").StripDiacritics();
                folderPath = folderPath.Replace('/', '\\').Trim('\\');

                var fullThumbPath = String.Format(@"{0}\{1}\{2}", m.Configuration.Instance.ThumbFullPath, folderPath, fileName);
                var fullUploadPath = String.Format(@"{0}\{1}\{2}", m.Configuration.Instance.UploadFullPath, folderPath, fileName);

                if (fileName.Contains("."))
                {
                    File.Delete(fullThumbPath);
                    File.Delete(fullUploadPath);
                }
                else
                {
                    Directory.Delete(fullThumbPath);
                    Directory.Delete(fullUploadPath);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}