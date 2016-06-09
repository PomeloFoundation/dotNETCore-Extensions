using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace System
{
    public enum Quality
    {
        Smallest,
        Medium,
        Best
    };

    public class Video : MediaFile
    {
        public readonly VideoInfo Info;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bytes">视频文件字节集</param>
        /// <param name="extension">扩展名（如.png）</param>
        public Video(byte[] bytes, string extension)
        {
            var fname = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + extension;
            System.IO.File.WriteAllBytes(fname, bytes);
            _Source = fname;
            Info = MediaHelper.GetImageInfo(fname);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src">视频源文件</param>
        public Video(string src)
        {
            if (!System.IO.File.Exists(src))
                throw new Exception(src + " Not Found.");
            _Source = src;
            Info = MediaHelper.GetVideoInfo(src);
        }

        /// <summary>
        /// 获取截图
        /// </summary>
        /// <param name="timeoff">时间（秒）</param>
        /// <returns></returns>
        public Image GetFrame(double timeoff = 1.0)
        {
            var fname = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(_Source);
            System.IO.File.WriteAllBytes((string)fname, MediaHelper.GetFrame((string)base._Source, timeoff));
            var ret = new Image(fname);
            ret.IsTemp = true;
            return ret;
        }

        public Task<Image> GetFrameAsync(double timeoff = 1.0)
        {
            return Task.Factory.StartNew(() => GetFrame(timeoff));
        }

        /// <summary>
        /// 获取截图
        /// </summary>
        /// <param name="width">宽(px)</param>
        /// <param name="height">高(px)</param>
        /// <param name="timeoff">时间（秒）</param>
        /// <returns></returns>
        public Image GetFrame(int width, int height, double timeoff = 1.0)
        {
            var fname = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(_Source);
            System.IO.File.WriteAllBytes((string)fname, MediaHelper.GetFrame((string)base._Source, width, height, timeoff));
            var ret = new Image(fname);
            ret.IsTemp = true;
            return ret;
        }

        public Task<Image> GetFrameAsync(int width, int height, double timeoff = 1.0)
        {
            return Task.Factory.StartNew(() => GetFrame(width, height, timeoff));
        }

        /// <summary>
        /// 抽取整个影片帧
        /// </summary>
        /// <param name="timeoff">时间（秒）</param>
        /// <returns></returns>
        public List<Image> GetFrames(int timeoff = 1)
        {
            var result = new List<Image>();
            for (var i = 0; i < Info.Duration.TotalSeconds; i += timeoff)
            {
                try
                {
                    var fname = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + ".jpg";
                    System.IO.File.WriteAllBytes(fname, MediaHelper.GetFrame(_Source, i));
                    var frame = new Image(fname);
                    frame.IsTemp = true;
                    result.Add(frame);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("System caught an exception:");
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                }
            }
            return result;
        }

        public Task<List<Image>> GetFramesAsync(int timeoff = 1)
        {
            return Task.Factory.StartNew(() => GetFrames(timeoff));
        }

        /// <summary>
        /// 抽取整个影片帧
        /// </summary>
        /// <param name="width">宽(px)</param>
        /// <param name="height">高(px)</param>
        /// <param name="timeoff">时间（秒）</param>
        /// <returns></returns>
        public List<Image> GetFrames(int width, int height, int timeoff = 1)
        {
            var result = new List<Image>();
            for (var i = 0; i <= Info.Duration.TotalSeconds; i += timeoff)
            {
                var fname = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + ".jpg";
                System.IO.File.WriteAllBytes(fname, MediaHelper.GetFrame(_Source, width, height, i));
                var frame = new Image(fname);
                frame.IsTemp = true;
                result.Add(frame);
            }
            return result;
        }

        public Task<List<Image>> GetFramesAsync(int width, int height, int timeoff = 1)
        {
            return Task.Factory.StartNew(() => GetFrames(width, height, timeoff));
        }

        /// <summary>
        /// 从影片中抽取指定数目的帧
        /// </summary>
        /// <param name="Count">数量</param>
        /// <returns></returns>
        public List<Image> GetFramesByCount(int Count)
        {
            var result = new List<Image>();
            double sec = (int)Info.Duration.TotalSeconds;
            var timeoff = sec / Count;
            for (double i = 0.0; i < Info.Duration.TotalSeconds; i += timeoff)
            {
                try
                {
                    var fname = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + ".jpg";
                    System.IO.File.WriteAllBytes(fname, MediaHelper.GetFrame(_Source, i));
                    var frame = new Image(fname);
                    frame.IsTemp = true;
                    result.Add(frame);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("System caught an exception:");
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                }
            }
            return result;
        }

        public Task<List<Image>> GetFramesByCountAsync(int Count)
        {
            return Task.Factory.StartNew(() => GetFramesByCount(Count));
        }

        /// <summary>
        /// 从影片中抽取指定数目的帧
        /// </summary>
        /// <param name="Count">数量</param>
        /// <param name="width">宽度(px)</param>
        /// <param name="height">高度(px)</param>
        /// <returns></returns>
        public List<Image> GetFramesByCount(int Count, int width, int height)
        {
            var result = new List<Image>();
            double sec = (int)Info.Duration.TotalSeconds;
            var timeoff = sec / Count;
            for (var i = 0.0; i < Info.Duration.TotalSeconds; i += timeoff)
            {
                try
                {
                    var fname = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + ".jpg";
                    System.IO.File.WriteAllBytes(fname, MediaHelper.GetFrame(_Source, width, height, i));
                    var frame = new Image(fname);
                    frame.IsTemp = true;
                    result.Add(frame);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("System caught an exception:");
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                }
            }
            return result;
        }

        public Task<List<Image>> GetFramesByCountAsync(int Count, int width, int height)
        {
            return Task.Factory.StartNew(() => GetFramesByCount(Count, width, height));
        }

        /// <summary>
        /// 视频文件字节集
        /// </summary>
        public override byte[] AllBytes
        {
            get { return System.IO.File.ReadAllBytes((string)base._Source); }
        }

        /// <summary>
        /// 向后连接一个视频
        /// </summary>
        /// <param name="file">目标文件</param>
        /// <returns></returns>
        public Video PushBack(Video file, Quality Quality)
        {
            var src = new List<string>();
            src.Add(_Source);
            src.Add(file._Source);
            var fname = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(_Source);
            try
            {
                MediaHelper.Concat(src, fname, Quality);
            }
            catch
            {
                return null;
            }
            var ret = new Video(fname);
            ret.IsTemp = true;
            return ret;
        }

        public Task<Video> PushBackAsync(Video file, Quality Quality)
        {
            return Task.Factory.StartNew(() => PushBack(file, Quality));
        }

        /// <summary>
        /// 向前连接视频文件
        /// </summary>
        /// <param name="file">目标文件</param>
        /// <returns></returns>
        public Video PushFront(Video file, Quality Quality)
        {
            var src = new List<string>();
            src.Add(file._Source);
            src.Add(_Source);
            var fname = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(_Source);
            try
            {
                MediaHelper.Concat(src, fname, Quality);
            }
            catch
            {
                return null;
            }
            var ret = new Video(fname);
            ret.IsTemp = true;
            return ret;
        }

        public Task<Video> PushFrontAsync(Video file, Quality Quality)
        {
            return Task.Factory.StartNew(() => PushFront(file, Quality));
        }

        /// <summary>
        /// 截取视频
        /// </summary>
        /// <param name="Begin">起始时间（秒）</param>
        /// <param name="Length">截取时间</param>
        /// <returns></returns>
        public Video Intercept(double Begin, TimeSpan Length, Quality Quality)
        {
            if (Begin + Length.TotalSeconds > Info.Duration.TotalSeconds)
                throw new Exception("超出了影片长度");
            var fname = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(_Source);
            var flag = false;
            try
            {
                flag = MediaHelper.Intercept(_Source, fname, Begin, Length, Quality);
            }
            catch
            {
                return null;
            }
            if (!flag) return null;
            var ret = new Video(fname);
            ret.IsTemp = true;
            return ret;
        }

        public Task<Video> InterceptAsync(double Begin, TimeSpan Length, Quality Quality)
        {
            return Task.Factory.StartNew(() => Intercept(Begin, Length, Quality));
        }

        /// <summary>
        /// 切分影片
        /// </summary>
        /// <param name="Interval">周期</param>
        /// <returns></returns>
        public List<Video> Split(TimeSpan Interval, Quality Quality)
        {
            var ret = new List<Video>();
            var TotalSeconds = Info.Duration.TotalSeconds;
            var Begin = 0;
            while (TotalSeconds > 0)
            {
                if (Interval.TotalSeconds > TotalSeconds)
                {
                    Interval = Interval - new TimeSpan(0, 0, System.Convert.ToInt32(TotalSeconds));
                }
                var fname = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(_Source);
                var tmp = MediaHelper.Intercept(_Source, fname, Begin, Interval, Quality);
                if (tmp)
                {
                    var tmp2 = new Video(fname);
                    tmp2.IsTemp = true;
                    ret.Add(tmp2);
                }
                TotalSeconds -= Interval.TotalSeconds;
                Begin += int.Parse(Interval.TotalSeconds.ToString());
            }
            return ret;
        }

        public Task<List<Video>> SplitAsync(TimeSpan Interval, Quality Quality)
        {
            return Task.Factory.StartNew(() => Split(Interval, Quality));
        }

        /// <summary>
        /// 转换格式
        /// </summary>
        /// <param name="format">扩展名（带有点）</param>
        /// <returns></returns>
        public Video Convert(string format, Quality Quality)
        {
            var fname = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + format;
            if (!MediaHelper.FormatConvert(Source, fname, Quality))
                return null;
            var ret = new Video(fname);
            ret.IsTemp = true;
            return ret;
        }

        public Task<Video> ConvertAsync(string format, Quality Quality)
        {
            return Task.Factory.StartNew(() => Convert(format, Quality));
        }

        /// <summary>
        /// 转换格式并限制尺寸
        /// </summary>
        /// <param name="format">扩展名（带有点）</param>
        /// <param name="width">宽度(px)</param>
        /// <param name="height">高度(px)</param>
        /// <returns></returns>
        public Video Convert(string format, int width, int height, Quality Quality)
        {
            var fname = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + format;
            if (!MediaHelper.FormatConvert(Source, fname, width, height, Quality))
                return null;
            var ret = new Video(fname);
            ret.IsTemp = true;
            return ret;
        }

        public Task<Video> ConvertAsync(string format, int width, int height, Quality Quality)
        {
            return Task.Factory.StartNew(() => Convert(format, width, height, Quality));
        }

        /// <summary>
        /// 压缩应聘啊
        /// </summary>
        /// <param name="width">宽度(px)</param>
        /// <param name="height">高度(px)</param>
        /// <returns></returns>
        public Video Compress(int width, int height, Quality Quality)
        {
            var fname = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(_Source);
            if (!MediaHelper.FormatConvert(Source, fname, width, height, Quality))
                return null;
            var ret = new Video(fname);
            ret.IsTemp = true;
            return ret;
        }

        public Task<Video> CompressAsync(int width, int height, Quality Quality)
        {
            return Task.Factory.StartNew(() => Compress(width, height, Quality));
        }

        /// <summary>
        /// 导出240P
        /// </summary>
        /// <returns></returns>
        public Video ExportTo240P()
        {
            var fname = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(_Source);
            if (!MediaHelper.FormatConvert(Source, fname, System.Convert.ToInt32(Info.Width * 240 / Info.Height), 240, Quality.Smallest))
                return null;
            var ret = new Video(fname);
            ret.IsTemp = true;
            return ret;
        }

        public Task<Video> ExportTo240PAsync()
        {
            return Task.Factory.StartNew(() => ExportTo240P());
        }


        /// <summary>
        /// 导出360P
        /// </summary>
        /// <returns></returns>
        public Video ExportTo360P()
        {
            var fname = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(_Source);
            if (!MediaHelper.FormatConvert(Source, fname, System.Convert.ToInt32(Info.Width * 360 / Info.Height), 360, Quality.Smallest))
                return null;
            var ret = new Video(fname);
            ret.IsTemp = true;
            return ret;
        }

        public Task<Video> ExportTo360PAsync()
        {
            return Task.Factory.StartNew(() => ExportTo360P());
        }

        /// <summary>
        /// 导出480P
        /// </summary>
        /// <returns></returns>
        public Video ExportTo480P()
        {
            var fname = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(_Source);
            if (!MediaHelper.FormatConvert(Source, fname, System.Convert.ToInt32(Info.Width * 480 / Info.Height), 480, Quality.Smallest))
                return null;
            var ret = new Video(fname);
            ret.IsTemp = true;
            return ret;
        }

        public Task<Video> ExportTo480PAsync()
        {
            return Task.Factory.StartNew(() => ExportTo480P());
        }

        /// <summary>
        /// 导出720P
        /// </summary>
        /// <returns></returns>
        public Video ExportTo720P()
        {
            var fname = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(_Source);
            if (!MediaHelper.FormatConvert(Source, fname, System.Convert.ToInt32(Info.Width * 720 / Info.Height), 720, Quality.Medium))
                return null;
            var ret = new Video(fname);
            ret.IsTemp = true;
            return ret;
        }

        public Task<Video> ExportTo720PAsync()
        {
            return Task.Factory.StartNew(() => ExportTo720P());
        }

        /// <summary>
        /// 导出1080P
        /// </summary>
        /// <returns></returns>
        public Video ExportTo1080P()
        {
            var fname = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(_Source);
            if (!MediaHelper.FormatConvert(Source, fname, System.Convert.ToInt32(Info.Width * 1080 / Info.Height), 1080, Quality.Best))
                return null;
            var ret = new Video(fname);
            ret.IsTemp = true;
            return ret;
        }

        public Task<Video> ExportTo1080PAsync()
        {
            return Task.Factory.StartNew(() => ExportTo1080P());
        }

        /// <summary>
        /// 导出2K
        /// </summary>
        /// <returns></returns>
        public Video ExportTo2K()
        {
            var fname = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(_Source);
            if (!MediaHelper.FormatConvert(Source, fname, System.Convert.ToInt32(Info.Width * 1440 / Info.Height), 1440, Quality.Best))
                return null;
            var ret = new Video(fname);
            ret.IsTemp = true;
            return ret;
        }

        public Task<Video> ExportTo2KPAsync()
        {
            return Task.Factory.StartNew(() => ExportTo2K());
        }

        /// <summary>
        /// 导出4K
        /// </summary>
        /// <returns></returns>
        public Video ExportTo4K()
        {
            var fname = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(_Source);
            if (!MediaHelper.FormatConvert(Source, fname, System.Convert.ToInt32(Info.Width * 2160 / Info.Height), 2160, Quality.Best))
                return null;
            var ret = new Video(fname);
            ret.IsTemp = true;
            return ret;
        }

        public Task<Video> ExportTo4KPAsync()
        {
            return Task.Factory.StartNew(() => ExportTo4K());
        }

        /// <summary>
        /// 截取影片
        /// </summary>
        /// <param name="Begin">开始时间（秒）</param>
        /// <param name="Length">截取时常</param>
        /// <returns></returns>
        public Video Intercept(int Begin, TimeSpan Length, Quality Quality)
        {
            var fname = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(_Source);
            if (!MediaHelper.Intercept(Source, fname, Begin, Length, Quality))
                return null;
            var ret = new Video(fname);
            ret.IsTemp = true;
            return ret;
        }

        public Task<Video> InterceptAsync(int Begin, TimeSpan Length, Quality Quality)
        {
            return Task.Factory.StartNew(() => Intercept(Begin, Length, Quality));
        }

        ~Video()
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
