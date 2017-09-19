using RNET.FileManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace RNET.FileManager
{
    public class FileManageModel
    {
        public FileManageModel()
        {
            items = new List<object>();
        }

        public string path { private get; set; }

        public string back
        {
            get
            {
                if (string.IsNullOrEmpty(path))
                    return string.Empty;

                return path.Substring(0, path.LastIndexOf("/"));
            }
        }

        public List<object> items { get; set; }

        public List<object> breadcrumbs
        {
            get
            {
                if (string.IsNullOrEmpty(path))
                    path = string.Empty;

                string[] fs = path.Split(new char[] { '/', '\\' });

                List<object> objs = new List<object>();

                string p = "";
                foreach (string f in fs)
                {
                    var n = f.Trim(new char[] { '/', '\\' });
                    if (!String.IsNullOrEmpty(n))
                    {
                        p = String.Format(@"{0}/{1}", p, n);
                        objs.Add(new { name = n, path = p });
                    }
                }

                return objs;
            }
        }
    }


    public class TreeNode
    {
        public string path { get; set; }
        public string label { get; set; }
        public IList<TreeNode> children { get; set; }
    }

    public class FileItem
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string ThumbImage { get; set; }
        public string Date { get; set; }

        public FileType FileType { get; set; }

        public object ToJson()
        {
            return new
            {
                Title = Name,
                Url = Url.ToLower(),
                Path = Path.ToLower(),
                Image = ThumbImage.ToLower(),
                Type = Convert.ToInt16(FileType),
                Checked = false
            };
        }
    }

    public class FileTypes
    {
        private FileTypes() { }

        public FileType this[string fileName]
        {
            get
            {
                var fileExtension = Path.GetExtension(fileName).ToLower().Trim('.');

                /// File
                if (Configuration.Instance.OtherExtensions.Contains(fileExtension))
                    return FileType.MISC;

                /// Music file
                if (Configuration.Instance.MusicExtensions.Contains(fileExtension))
                    return FileType.MUSIC;

                ///Video
                if (Configuration.Instance.VideoExtensions.Contains(fileExtension))
                    return FileType.VIDEO;

                /// Image file
                if (Configuration.Instance.ImageExtensions.Contains(fileExtension))
                    return FileType.PICTURE;

                return FileType.MISC;
            }
        }

        private static FileTypes _Instance = null;
        public static FileTypes Instance
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
            internal static readonly FileTypes Initialize = new FileTypes();
        }
    }

    public enum FileType
    {
        FOLDER = 1,
        PICTURE = 2,
        MISC = 3,
        VIDEO = 4,
        MUSIC = 5
    }
}