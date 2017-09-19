using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace RNET.FileManager.Models
{
    public class FileManagerSection : ConfigurationSection
    {
        [ConfigurationProperty("UserInfo")]
        public UserInfoElement UserInfo
        {
            get
            {
                return (UserInfoElement)this["UserInfo"];
            }
            set
            {
                this["UserInfo"] = value;
            }
        }


        [ConfigurationProperty("RootURL")]
        public RootURLElement RootURL
        {
            get
            {
                return (RootURLElement)this["RootURL"];
            }
            set
            {
                this["RootURL"] = value;
            }
        }

        [ConfigurationProperty("RootPath")]
        public RootPathElement RootPath
        {
            get
            {
                return (RootPathElement)this["RootPath"];
            }
            set
            {
                this["RootPath"] = value;
            }
        }

        [ConfigurationProperty("PrivateUser")]
        public PrivateUserElement PrivateUser
        {
            get
            {
                return (PrivateUserElement)this["PrivateUser"];
            }
            set
            {
                this["PrivateUser"] = value;
            }
        }

        [ConfigurationProperty("AcceptUpload")]
        public AcceptUploadElement AcceptUpload
        {
            get
            {
                return (AcceptUploadElement)this["AcceptUpload"];
            }
            set
            {
                this["AcceptUpload"] = value;
            }
        }

        [ConfigurationProperty("AcceptDelete")]
        public AcceptDeleteElement AcceptDelete
        {
            get
            {
                return (AcceptDeleteElement)this["AcceptDelete"];
            }
            set
            {
                this["AcceptDelete"] = value;
            }
        }

        [ConfigurationProperty("MaxSize")]
        public MaxSizeElement MaxSize
        {
            get
            {
                return (MaxSizeElement)this["MaxSize"];
            }
            set
            {
                this["MaxSize"] = value;
            }
        }

        [ConfigurationProperty("ImageResize")]
        public ImageResizeElement ImageResize
        {
            get
            {
                return (ImageResizeElement)this["ImageResize"];
            }
            set
            {
                this["ImageResize"] = value;
            }
        }

        [ConfigurationProperty("ThumbPath")]
        public ThumbPathElement ThumbPath
        {
            get
            {
                return (ThumbPathElement)this["ThumbPath"];
            }
            set
            {
                this["ThumbPath"] = value;
            }
        }

        [ConfigurationProperty("UploadPath")]
        public UploadPathElement UploadPath
        {
            get
            {
                return (UploadPathElement)this["UploadPath"];
            }
            set
            {
                this["UploadPath"] = value;
            }
        }

        [ConfigurationProperty("ImageExtension")]
        public ImageExtensionElement ImageExtension
        {
            get
            {
                return (ImageExtensionElement)this["ImageExtension"];
            }
            set
            {
                this["ImageExtension"] = value;
            }
        }

        [ConfigurationProperty("VideoExtension")]
        public VideoExtensionElement VideoExtension
        {
            get
            {
                return (VideoExtensionElement)this["VideoExtension"];
            }
            set
            {
                this["VideoExtension"] = value;
            }
        }

        [ConfigurationProperty("MusicExtension")]
        public MusicExtensionElement MusicExtension
        {
            get
            {
                return (MusicExtensionElement)this["MusicExtension"];
            }
            set
            {
                this["MusicExtension"] = value;
            }
        }

        [ConfigurationProperty("OtherExtension")]
        public OtherExtensionElement OtherExtension
        {
            get
            {
                return (OtherExtensionElement)this["OtherExtension"];
            }
            set
            {
                this["OtherExtension"] = value;
            }
        }

        [ConfigurationProperty("Theme")]
        public ThemeElement Theme
        {
            get
            {
                return (ThemeElement)this["Theme"];
            }
            set
            {
                this["Theme"] = value;
            }
        }
    }

    public class UserInfoElement : ConfigurationElement
    {
        [ConfigurationProperty("name")]
        public string name
        {
            get
            {
                try
                {
                    return Convert.ToString(this["name"]).ToLower();
                }
                catch
                {
                    return "tinyfile";
                }
            }
            set
            {
                this["name"] = value.ToLower();
            }
        }

        [ConfigurationProperty("password")]
        public string password
        {
            get
            {
                try
                {
                    return Convert.ToString(this["password"]).ToLower();
                }
                catch
                {
                    return "tinyfile";
                }
            }
            set
            {
                this["password"] = value.ToLower();
            }
        }
    }

    public class ThemeElement : ConfigurationElement
    {
        [ConfigurationProperty("value", DefaultValue = "min", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{};'\"|.,/", MinLength = 3, MaxLength = 7)]
        public string Value
        {
            get
            {
                try
                {
                    return Convert.ToString(this["value"]).ToLower();
                }
                catch
                {
                    return "min";
                }
            }
            set
            {
                this["value"] = value.ToLower();
            }
        }
    }

    public class RootPathElement : ConfigurationElement
    {
        [ConfigurationProperty("value", DefaultValue = "\\", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{};'\"|", MinLength = 1, MaxLength = 60)]
        public string Value
        {
            get
            {
                try
                {
                    return Convert.ToString(this["value"]).ToLower().Trim('\\');
                }
                catch
                {
                    return String.Empty;
                }
            }
            set
            {
                this["value"] = value.ToLower().Trim('\\');
            }
        }
    }

    public class RootURLElement : ConfigurationElement
    {
        [ConfigurationProperty("value", DefaultValue = "/", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{};'\"|", MinLength = 1, MaxLength = 60)]
        public string Value
        {
            get
            {
                try
                {
                    return Convert.ToString(this["value"]).ToLower().Trim('/');
                }
                catch
                {
                    return String.Empty;
                }
            }
            set
            {
                this["value"] = value.ToLower().Trim('/');
            }
        }
    }

    public class PrivateUserElement : ConfigurationElement
    {
        [ConfigurationProperty("value", DefaultValue = false, IsRequired = true)]
        public bool Value
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(this["value"]);
                }
                catch
                {
                    return true;
                }
            }
            set
            {
                this["value"] = value;
            }
        }
    }

    public class AcceptUploadElement : ConfigurationElement
    {
        [ConfigurationProperty("value", DefaultValue = false, IsRequired = true)]
        public bool Value
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(this["value"]);
                }
                catch
                {
                    return false;
                }
            }
            set
            {
                this["value"] = value;
            }
        }
    }

    public class AcceptDeleteElement : ConfigurationElement
    {
        [ConfigurationProperty("value", DefaultValue = false, IsRequired = true)]
        public bool Value
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(this["value"]);
                }
                catch
                {
                    return false;
                }
            }
            set
            {
                this["value"] = value;
            }
        }
    }

    public class MaxSizeElement : ConfigurationElement
    {
        [ConfigurationProperty("value", DefaultValue = 4.3d, IsRequired = true)]
        public double Value
        {
            get
            {
                try
                {
                    return Convert.ToDouble(this["value"]);
                }
                catch
                {
                    return 4.3d;
                }
            }
            set
            {
                this["value"] = value;
            }
        }
    }

    public class ImageResizeElement : ConfigurationElement
    {
        [ConfigurationProperty("width", DefaultValue = 650, IsRequired = true)]
        public int Width
        {
            get
            {
                try
                {
                    return Convert.ToInt16(this["width"]);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                this["width"] = value;
            }
        }

        [ConfigurationProperty("height", DefaultValue = 650, IsRequired = true)]
        public int Height
        {
            get
            {
                try
                {
                    return Convert.ToInt16(this["height"]);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                this["height"] = value;
            }
        }
    }

    public class ThumbPathElement : ConfigurationElement
    {
        [ConfigurationProperty("value", DefaultValue = @"\Uploads\Thumbs\", IsRequired = true)]
        public string Value
        {
            get
            {
                try
                {
                    return Convert.ToString(this["value"]).ToLower();
                }
                catch
                {
                    return String.Empty;
                }
            }
            set
            {
                this["value"] = value.ToLower();
            }
        }
    }

    public class UploadPathElement : ConfigurationElement
    {
        [ConfigurationProperty("value", DefaultValue = @"\Uploads\Images\", IsRequired = true)]
        public string Value
        {
            get
            {
                try
                {
                    return Convert.ToString(this["value"]).ToLower();
                }
                catch
                {
                    return String.Empty;
                }
            }
            set
            {
                this["value"] = value.ToLower();
            }
        }
    }

    public class MusicExtensionElement : ConfigurationElement
    {
        [ConfigurationProperty("value", DefaultValue = @"'mp3', 'm4a', 'ac3', 'aiff', 'mid'", IsRequired = true)]
        public string Value
        {
            get
            {
                try
                {
                    return Convert.ToString(this["value"]).ToLower();
                }
                catch
                {
                    return String.Empty;
                }
            }
            set
            {
                this["value"] = value.ToLower();
            }
        }

        public string[] Values
        {
            get
            {
                return Value.Replace("'", String.Empty).Replace(" ", String.Empty).Split(new char[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }

    public class VideoExtensionElement : ConfigurationElement
    {
        [ConfigurationProperty("value", DefaultValue = @"'mov', 'mpeg', 'mp4', 'avi', 'mpg','wma'", IsRequired = true)]
        public string Value
        {
            get
            {
                try
                {
                    return Convert.ToString(this["value"]).ToLower();
                }
                catch
                {
                    return String.Empty;
                }
            }
            set
            {
                this["value"] = value.ToLower();
            }
        }

        public string[] Values
        {
            get
            {
                return Value.Replace("'", String.Empty).Replace(" ", String.Empty).Split(new char[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }

    public class ImageExtensionElement : ConfigurationElement
    {
        [ConfigurationProperty("value", DefaultValue = @"'jpg', 'jpeg', 'png', 'gif', 'bmp', 'tiff'", IsRequired = true)]
        public string Value
        {
            get
            {
                try
                {
                    return Convert.ToString(this["value"]).ToLower();
                }
                catch
                {
                    return String.Empty;
                }
            }
            set
            {
                this["value"] = value.ToLower();
            }
        }

        public string[] Values
        {
            get
            {
                return Value.Replace("'", String.Empty).Replace(" ", String.Empty).Split(new char[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }

    public class OtherExtensionElement : ConfigurationElement
    {
        [ConfigurationProperty("value", DefaultValue = @"'doc', 'docx', 'pdf', 'xls', 'xlsx', 'txt', 'csv','html','psd','sql','log','fla','xml','ade','adp','ppt','pptx'", IsRequired = true)]
        public string Value
        {
            get
            {
                try
                {
                    return Convert.ToString(this["value"]).ToLower();
                }
                catch
                {
                    return String.Empty;
                }
            }
            set
            {
                this["value"] = value.ToLower();
            }
        }

        public string[] Values
        {
            get
            {
                return Value.Replace("'", String.Empty).Replace(" ", String.Empty).Split(new char[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }
}