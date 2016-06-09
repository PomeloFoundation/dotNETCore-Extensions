using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace System
{
    public class Image : MediaFile
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bytes">图片文件字节集</param>
        /// <param name="extension">扩展名（如.png）</param>
        public Image(byte[] bytes, string extension)
        {
            var fname = Path.GetTempPath() + "codecomb_" + Guid.NewGuid().ToString().Replace("-", "") + extension;
            System.IO.File.WriteAllBytes(fname, bytes);
            _Source = fname;
            var info = MediaHelper.GetImageInfo(fname);
            Width = info.Width;
            Height = info.Height;
        }

        public Image(string src)
        {
            _Source = src;
            var info = MediaHelper.GetImageInfo(src);
            Width = info.Width;
            Height = info.Height;
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public override byte[] AllBytes
        {
            get
            {
                return System.IO.File.ReadAllBytes((string)base._Source);
            }
        }

        /// <summary>
        /// 转换图片格式
        /// </summary>
        /// <param name="Extension">目标图片扩展名，如(.png)</param>
        /// <returns></returns>
        public Image Convert(string Extension)
        {
            var fname = Path.GetTempPath() + "codecomb_" + Guid.NewGuid().ToString().Replace("-", "") + Extension;
            if (!MediaHelper.ImageFormatConvert(_Source, fname))
                return null;
            var ret = new Image(fname);
            ret.IsTemp = true;
            return ret;
        }

        public Task<Image> ConvertAsync(string Extension)
        {
            return Task.Factory.StartNew(() => Convert(Extension));
        }

        /// <summary>
        /// 转换图片格式
        /// </summary>
        /// <param name="Extension">目标图片扩展名，如(.png)</param>
        /// <param name="Width">宽度（px）</param>
        /// <param name="Height">高度（px）</param>
        /// <returns></returns>
        public Image Convert(string Extension, int Width, int Height)
        {
            var fname = Path.GetTempPath() + "codecomb_" + Guid.NewGuid().ToString().Replace("-", "") + Extension;
            if (!MediaHelper.ImageFormatConvert(_Source, fname, Width, Height))
                return null;
            var ret = new Image(fname);
            ret.IsTemp = true;
            return ret;
        }

        public Task<Image> ConvertAsync(string Extension, int Width, int Height)
        {
            return Task.Factory.StartNew(() => Convert(Extension, Width, Height));
        }

        /// <summary>
        /// 转换图片格式
        /// </summary>
        /// <param name="Extension">扩展名（如.png）</param>
        /// <param name="Width">宽度（px）</param>
        /// <returns></returns>
        public Image ConvertByWidth(string Extension, int Width)
        {
            var fname = Path.GetTempPath() + "codecomb_" + Guid.NewGuid().ToString().Replace("-", "") + Extension;
            if (!MediaHelper.ImageFormatConvert(_Source, fname, Width, Width * this.Height / this.Width))
                return null;
            var ret = new Image(fname);
            ret.IsTemp = true;
            return ret;
        }

        public Task<Image> ConvertByWidthAsync(string Extension, int Width, int Height)
        {
            return Task.Factory.StartNew(() => ConvertByWidth(Extension, Width));
        }

        /// <summary>
        /// 转换图片格式
        /// </summary>
        /// <param name="Extension">扩展名（如.png）</param>
        /// <param name="Height">高度（px）</param>
        /// <returns></returns>
        public Image ConvertByHeight(string Extension, int Height)
        {
            var fname = Path.GetTempPath() + "codecomb_" + Guid.NewGuid().ToString().Replace("-", "") + Extension;
            if (!MediaHelper.ImageFormatConvert(_Source, fname, Height * this.Width / this.Height, Height))
                return null;
            var ret = new Image(fname);
            ret.IsTemp = true;
            return ret;
        }

        public Task<Image> ConvertByHeightAsync(string Extension, int Height)
        {
            return Task.Factory.StartNew(() => ConvertByHeight(Extension, Height));
        }

        /// <summary>
        /// 调整大小
        /// </summary>
        /// <param name="Width">宽度（px）</param>
        /// <param name="Height">高度（px）</param>
        /// <returns></returns>
        public Image Resize(int Width, int Height)
        {
            var fname = Path.GetTempPath() + "codecomb_" + Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(_Source);
            if (!MediaHelper.ImageFormatConvert(_Source, fname, Width, Height))
                return null;
            var ret = new Image(fname);
            ret.IsTemp = true;
            return ret;
        }

        public Task<Image> ResizeAsync(int Width, int Height)
        {
            return Task.Factory.StartNew(() => Resize(Width, Height));
        }

        /// <summary>
        /// 根据宽度等比例调整尺寸
        /// </summary>
        /// <param name="Width">宽度（px）</param>
        /// <returns></returns>
        public Image ResizeByWidth(int Width)
        {
            var fname = Path.GetTempPath() + "codecomb_" + Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(_Source);
            if (!MediaHelper.ImageFormatConvert(_Source, fname, Width, Width * this.Height / this.Width))
                return null;
            var ret = new Image(fname);
            ret.IsTemp = true;
            return ret;
        }

        public Task<Image> ResizeByWidthAsync(int Width)
        {
            return Task.Factory.StartNew(() => ResizeByWidth(Width));
        }

        /// <summary>
        /// 根据高度等比例调整尺寸
        /// </summary>
        /// <param name="Height">高度（px）</param>
        /// <returns></returns>
        public Image ResizeByHeight(int Height)
        {
            var fname = Path.GetTempPath() + "codecomb_" + Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(_Source);
            if (!MediaHelper.ImageFormatConvert(_Source, fname, Height * this.Width / this.Height, Height))
                return null;
            var ret = new Image(fname);
            ret.IsTemp = true;
            return ret;
        }

        public Task<Image> ResizeByHeightAsync(int Height)
        {
            return Task.Factory.StartNew(() => ResizeByHeight(Height));
        }

        ~Image()
        {
            if (IsTemp)
            {
                try
                {
                    System.IO.File.Delete((string)base._Source);
                }
                catch
                {
                }
            }
        }
    }
}
