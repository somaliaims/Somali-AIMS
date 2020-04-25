using AIMS.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.APIs.Helpers
{
    public interface IImageHelper
    {
        /// <summary>
        /// Checks valid image format
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        ImageValidityModel IsImageFormatValid(IFormFile file);
    }

    public class ImageHelper : IImageHelper
    {
        //2MB in bytes
        readonly long MAX_IMAGE_SIZE = 2097152;

        public ImageHelper()
        {
        }

        public enum ImageFormat
        {
            bmp,
            jpeg,
            gif,
            tiff,
            png,
            unknown
        }

        public ImageFormat GetImageFormat(byte[] bytes)
        {
            var bmp = Encoding.ASCII.GetBytes("BM");     // BMP
            var gif = Encoding.ASCII.GetBytes("GIF");    // GIF
            var png = new byte[] { 137, 80, 78, 71 };    // PNG
            var tiff = new byte[] { 73, 73, 42 };         // TIFF
            var tiff2 = new byte[] { 77, 77, 42 };         // TIFF
            var jpeg = new byte[] { 255, 216, 255, 224 }; // jpeg
            var jpeg2 = new byte[] { 255, 216, 255, 225 }; // jpeg canon

            if (bmp.SequenceEqual(bytes.Take(bmp.Length)))
                return ImageFormat.bmp;

            if (gif.SequenceEqual(bytes.Take(gif.Length)))
                return ImageFormat.gif;

            if (png.SequenceEqual(bytes.Take(png.Length)))
                return ImageFormat.png;

            if (tiff.SequenceEqual(bytes.Take(tiff.Length)))
                return ImageFormat.tiff;

            if (tiff2.SequenceEqual(bytes.Take(tiff2.Length)))
                return ImageFormat.tiff;

            if (jpeg.SequenceEqual(bytes.Take(jpeg.Length)))
                return ImageFormat.jpeg;

            if (jpeg2.SequenceEqual(bytes.Take(jpeg2.Length)))
                return ImageFormat.jpeg;

            return ImageFormat.unknown;
        }

        public ImageValidityModel IsImageFormatValid(IFormFile file)
        {
            ImageValidityModel validityModel = new ImageValidityModel();
            byte[] fileBytes;
            long fileSizeInBytes = 0;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
                fileSizeInBytes = ms.Length;
            }
            validityModel.IsImageValid = (GetImageFormat(fileBytes) != ImageFormat.unknown);
            validityModel.IsImageSizeValid = (fileSizeInBytes <= MAX_IMAGE_SIZE);
            return validityModel;
        }

    }
}
