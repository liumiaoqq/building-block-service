using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouJu.Infrastructure.Images
{
    public static  class ImageType
    {
        public static string JPG = "jpg";


        public static string BMP = "bmp";

        public static string GIF = "gif";

        public static string JPEG = "jpeg";

        public static string PNG = "png";
        public static string[] GetALL()
        {
            return new[] {JPG,BMP,GIF,JPEG, PNG }; 
        }
    }
}
