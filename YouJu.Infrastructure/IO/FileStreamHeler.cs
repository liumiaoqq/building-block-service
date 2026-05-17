using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YouJu.Infrastructure.IO
{
    public static  class FileStreamHeler
    {

        public static Stream GetFileStreamByUrl(this string url)
        {
            WebRequest request = null;
            WebResponse response = null;
          
            try
            {
                 request = WebRequest.Create(url);
                 response = request.GetResponse();
               return response.GetResponseStream();
               
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (response is not null)
                {
                    response.Dispose();
                }
               
            
            }
        }

        public static string ImageToBase64(this Image _image)
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                if (ImageFormatGuidToString(_image.RawFormat) == null)
                {
                    _image.Save(ms, ImageFormat.Png);
                }
                else
                {
                    _image.Save(ms, _image.RawFormat);
                }
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                return Convert.ToBase64String(arr);
            }
            catch
            {
                return null;
            }
            finally
            {
                ms.Close();
            }
        }


        // 用于检查图像格式
        public static string ImageFormatGuidToString(this ImageFormat _format)
        {
            if (_format.Guid == ImageFormat.Bmp.Guid)
            {
                return "bmp";
            }
            else if (_format.Guid == ImageFormat.Gif.Guid)
            {
                return "gif";
            }
            else if (_format.Guid == ImageFormat.Jpeg.Guid)
            {
                return "jpg";
            }
            else if (_format.Guid == ImageFormat.Png.Guid)
            {
                return "png";
            }
            else if (_format.Guid == ImageFormat.Icon.Guid)
            {
                return "ico";
            }
            else if (_format.Guid == ImageFormat.Emf.Guid)
            {
                return "emf";
            }
            else if (_format.Guid == ImageFormat.Exif.Guid)
            {
                return "exif";
            }
            else if (_format.Guid == ImageFormat.Tiff.Guid)
            {
                return "tiff";
            }
            else if (_format.Guid == ImageFormat.Wmf.Guid)
            {
                return "wmf";
            }
            else
            {
                return null;
            }
        }
        public static Image GetFileStearmByUrl(this string url)
        {
            WebRequest request = null;
            WebResponse response = null;
            Stream imgstream = null;
            try
            {
                request = WebRequest.Create(url);
                response = request.GetResponse();
                imgstream = response.GetResponseStream();
                return System.Drawing.Image.FromStream(imgstream);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (response is not null)
                {
                    response.Dispose();
                }
                if (imgstream is not null)
                {
                    imgstream.Dispose();
                }

            }
        }

    }
}
