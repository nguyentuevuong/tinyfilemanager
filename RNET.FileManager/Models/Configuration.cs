using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace RNET.FileManager.Models
{
    public class Configuration
    {
        private FileManagerSection fileManagerSection;
        private Configuration()
        {
            fileManagerSection = (FileManagerSection)ConfigurationManager.GetSection("fileManager.Profiles");
        }

        public string[] UserInfo
        {
            get
            {
                return new[] { fileManagerSection.UserInfo.name, fileManagerSection.UserInfo.password };
            }
        }

        public string Theme
        {
            get
            {
                return fileManagerSection.Theme.Value;
            }
        }

        public double MaxSize
        {
            get
            {
                return fileManagerSection.MaxSize.Value;
            }
        }

        public int ImageResizeWidth
        {
            get
            {
                return fileManagerSection.ImageResize.Width;
            }
        }

        public int ImageResizeHeight
        {
            get
            {
                return fileManagerSection.ImageResize.Height;
            }
        }

        public bool PrivateUser
        {
            get
            {
                return fileManagerSection.PrivateUser.Value;
            }
        }

        public bool AcceptUpload
        {
            get
            {
                return fileManagerSection.AcceptUpload.Value;
            }
        }

        public bool AcceptDelete
        {
            get
            {
                return fileManagerSection.AcceptDelete.Value;
            }
        }

        public string[] MusicExtensions
        {
            get
            {
                return fileManagerSection.MusicExtension.Values;
            }
        }

        public string[] VideoExtensions
        {
            get
            {
                return fileManagerSection.VideoExtension.Values;
            }
        }

        public string[] ImageExtensions
        {
            get
            {
                return fileManagerSection.ImageExtension.Values;
            }
        }

        public string[] OtherExtensions
        {
            get
            {
                return fileManagerSection.OtherExtension.Values;
            }
        }

        public string[] AllExtensions
        {
            get
            {
                return fileManagerSection.ImageExtension.Values
                    .Concat(fileManagerSection.MusicExtension.Values)
                    .Concat(fileManagerSection.VideoExtension.Values)
                    .Concat(fileManagerSection.OtherExtension.Values)
                    .ToArray();
            }
        }

        public string BasePath
        {
            get
            {
                if (!String.IsNullOrEmpty(fileManagerSection.RootPath.Value))
                {
                    if (fileManagerSection.RootPath.Value.Contains(".."))
                    {
                        int backslash = fileManagerSection.RootPath.Value.Split(new string[] { ".." }, StringSplitOptions.None).Count() - 1;

                        var rootPath1 = HttpContext.Current.Server.MapPath("~/").Trim('\\');
                        var rootPath2 = fileManagerSection.RootPath.Value.Substring(fileManagerSection.RootPath.Value.LastIndexOf("..") + 2);
                        while (backslash > 0)
                        {
                            rootPath1 = rootPath1.EndsWith(":") ? rootPath1 : rootPath1.Substring(0, rootPath1.LastIndexOf('\\'));
                            backslash--;
                        }
                        return String.Format("{0}\\{1}", rootPath1, rootPath2).Trim('\\').ToLower();
                    }

                    return fileManagerSection.RootPath.Value.Trim('\\').ToLower();
                }
                else
                    return HttpContext.Current.Server.MapPath("~/").Trim('\\').ToLower();
            }
        }

        public string ThumbPath
        {
            get
            {
                return String.Format("{0}\\{1}", fileManagerSection.ThumbPath.Value, PrivateUser ? UserHash : "").TrimEnd('\\').ToLower();
            }
        }

        public string UploadPath
        {
            get
            {
                return String.Format("{0}\\{1}", fileManagerSection.UploadPath.Value, PrivateUser ? UserHash : "").TrimEnd('\\').ToLower();
            }
        }

        public string ThumbFullPath
        {
            get
            {
                var _thumbPath = String.Format("{0}\\{1}", BasePath, ThumbPath);

                _thumbPath = Regex.Replace(_thumbPath.Replace("\\", "/"), "/{2,}", "/").Trim('/').Replace("/", "\\").ToLower();

                if (!Directory.Exists(_thumbPath))
                    Directory.CreateDirectory(_thumbPath);

                return _thumbPath;
            }
        }

        public string UploadFullPath
        {
            get
            {
                var _uploadPath = String.Format("{0}\\{1}", BasePath, UploadPath);

                _uploadPath = Regex.Replace(_uploadPath.Replace("\\", "/"), "/{2,}", "/").Trim('/').Replace("/", "\\").ToLower();

                if (!Directory.Exists(_uploadPath))
                    Directory.CreateDirectory(_uploadPath);

                return _uploadPath;
            }
        }

        public string BaseURL
        {
            get
            {
                if (!String.IsNullOrEmpty(fileManagerSection.RootURL.Value))
                    return fileManagerSection.RootURL.Value.TrimEnd('/');
                else
                    return String.Format("{0}://{1}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority.TrimEnd('/'));
            }
        }

        public string ThumbURL
        {
            get
            {
                return String.Format("{0}/{1}/{2}", !String.IsNullOrEmpty(fileManagerSection.RootURL.Value) ? BaseURL : "", fileManagerSection.ThumbPath.Value.Replace("\\", "/").Trim('/'), PrivateUser ? UserHash : "").TrimEnd('/');
            }
        }

        public string UploadURL
        {
            get
            {
                return String.Format("{0}/{1}/{2}", !String.IsNullOrEmpty(fileManagerSection.RootURL.Value) ? BaseURL : "", fileManagerSection.UploadPath.Value.Replace("\\", "/").Trim('/'), PrivateUser ? UserHash : "").TrimEnd('/');
            }
        }

        private string UserHash
        {
            get
            {
                HashAlgorithm hashAlgorithm = new MD5CryptoServiceProvider();

                byte[] buffer = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(HttpContext.Current.User.Identity.Name));

                StringBuilder builder = new StringBuilder();
                foreach (byte buf in buffer)
                    builder.AppendFormat("{0:x2}", buf);

                return builder.ToString();
            }
        }

        public object ToJson()
        {
            if (UserInfo[1] == String.Empty)
                HttpContext.Current.Session["userInfo"] = "tinyfile";

            return new
            {
                size = MaxSize,
                upload = AcceptUpload,
                delete = AcceptDelete,
                extensions = AllExtensions,
                root = UploadPath.Replace("\\", "/").TrimEnd('/'),
                auth = HttpContext.Current.Session["userInfo"] != null
            };
        }

        private static Configuration _Instance = null;

        public static Configuration Instance
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
            internal static readonly Configuration Initialize = new Configuration();
        }
    }
}