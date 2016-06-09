using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Pomelo.Package;

namespace System
{
    public static class MediaHelper
    {
        public static string[] Args =
        {
            "-qscale 32 -ab 56 -ar 11025 -b 500k  -r 15",
            "-qscale 8 -ab 72 -ar 22050 -b 800k  -r 25.97",
            "-qscale 2 -ab 96 -ar 44100 -b 1500k  -r 29.97"
        };

        private static string FfmpegPath;

        static MediaHelper()
        {
            var TmpPath = Path.GetTempPath();

            if (System.Platform.OS == OSType.Windows)
            {
                FfmpegPath = TmpPath + @"ffmpeg.exe";
                if (!File.Exists(FfmpegPath))
                {
                    Download.DownloadAndExtract("https://github.com/PomeloResources/Ffmpeg/archive/windows.zip",
                        Tuple.Create("Ffmpeg-windows/ffmpeg.exe", FfmpegPath)).Wait();
                }
            }
            else if (System.Platform.OS == OSType.OSX)
            {
                FfmpegPath = TmpPath + @"ffmpeg";
                if (!File.Exists(FfmpegPath))
                {
                    Download.DownloadAndExtract("https://github.com/PomeloResources/Ffmpeg/archive/osx.zip",
                        Tuple.Create("Ffmpeg-osx/ffmpeg", FfmpegPath)).Wait();
                }
            }
            else
            {
                FfmpegPath = TmpPath + @"ffmpeg";
                if (!File.Exists(FfmpegPath))
                {
                    Download.DownloadAndExtract("https://github.com/PomeloResources/Ffmpeg/archive/linux.zip",
                        Tuple.Create("Ffmpeg-linux/ffmpeg", FfmpegPath)).Wait();
                }
            }
            if (System.Platform.OS != OSType.Windows)
            {
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    FileName = "chmod",
                    Arguments = "u+x \"" + FfmpegPath + "\"",
                    RedirectStandardError = true
                };
                Console.WriteLine("chmoding");
                p.Start();
                p.WaitForExit();
                Console.WriteLine(p.StandardError.ReadToEnd());
            }
        }

        /// <summary>
        /// 尝试将一个视频文件转换格式并压缩
        /// </summary>
        /// <param name="src">视频源文件</param>
        /// <param name="dest">目标文件</param>
        /// <param name="width">宽度(px)</param>
        /// <param name="height">高度(px)</param>
        /// <returns></returns>
        public static bool FormatConvert(string src, string dest, int width, int height, Quality Quality)
        {
            if (!System.IO.File.Exists(FfmpegPath))
            {
                throw new Exception("Ffmpeg not found: " + FfmpegPath);
            }
            if (!System.IO.File.Exists(src))
            {
                throw new Exception("The video file is not exist: " + src);
            }
            System.Diagnostics.ProcessStartInfo FilestartInfo = new System.Diagnostics.ProcessStartInfo(FfmpegPath);
            FilestartInfo.Arguments = " -i \"" + src + "\" " + Args[Convert.ToInt32(Quality)] + " -s " + width + "x" + height + " \"" + dest + "\"";
            try
            {
                var proc = System.Diagnostics.Process.Start(FilestartInfo);
                proc.WaitForExit();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public static bool ImageFormatConvert(string src, string dest, int width, int height)
        {
            if (!System.IO.File.Exists(FfmpegPath))
            {
                throw new Exception("Ffmpeg not found: " + FfmpegPath);
            }
            if (!System.IO.File.Exists(src))
            {
                throw new Exception("The video file is not exist: " + src);
            }
            System.Diagnostics.ProcessStartInfo FilestartInfo = new System.Diagnostics.ProcessStartInfo(FfmpegPath);
            FilestartInfo.Arguments = " -i \"" + src + "\" " + " -f image2 -s " + width + "x" + height + " \"" + dest + "\"";
            try
            {
                var proc = System.Diagnostics.Process.Start(FilestartInfo);
                proc.WaitForExit();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 尝试转换视频格式
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public static bool FormatConvert(string src, string dest, Quality Quality)
        {
            if (!System.IO.File.Exists(FfmpegPath))
            {
                throw new Exception("Ffmpeg not found: " + FfmpegPath);
            }
            if (!System.IO.File.Exists(src))
            {
                throw new Exception("The video file is not exist: " + src);
            }
            System.Diagnostics.ProcessStartInfo FilestartInfo = new System.Diagnostics.ProcessStartInfo(FfmpegPath);
            FilestartInfo.Arguments = " -i \"" + src + "\" " + Args[Convert.ToInt32(Quality)] + " \"" + dest + "\"";
            try
            {
                var proc = System.Diagnostics.Process.Start(FilestartInfo);
                proc.WaitForExit();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public static bool ImageFormatConvert(string src, string dest)
        {
            if (!System.IO.File.Exists(FfmpegPath))
            {
                throw new Exception("Ffmpeg not found: " + FfmpegPath);
            }
            if (!System.IO.File.Exists(src))
            {
                throw new Exception("The video file is not exist: " + src);
            }
            System.Diagnostics.ProcessStartInfo FilestartInfo = new System.Diagnostics.ProcessStartInfo(FfmpegPath);
            FilestartInfo.Arguments = " -i \"" + src + "\" -f image2 " + " \"" + dest + "\"";
            try
            {
                var proc = System.Diagnostics.Process.Start(FilestartInfo);
                proc.WaitForExit();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 尝试获取视频某一秒的截图
        /// </summary>
        /// <param name="src">视频源文件</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="timeoff">截屏时间</param>
        /// <returns>返回图片二进制文件</returns>
        public static byte[] GetFrame(string src, int width, int height, double timeoff = 1.0)
        {
            if (!System.IO.File.Exists(FfmpegPath))
            {
                throw new Exception("Ffmpeg not found: " + FfmpegPath);
            }
            if (!System.IO.File.Exists(src))
            {
                throw new Exception("The video file is not exist: " + src);
            }
            var _timeoff = timeoff.ToString("0.000");
            System.Diagnostics.ProcessStartInfo FilestartInfo = new System.Diagnostics.ProcessStartInfo(FfmpegPath);
            var PictureName = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + ".jpg";
            FilestartInfo.Arguments = String.Format("-i \"{0}\" -y -f image2 -ss {1} -t 0.001 -s {2}x{3} '{4}'", src, _timeoff, width, height, PictureName);
            var proc = System.Diagnostics.Process.Start(FilestartInfo);
            proc.WaitForExit();
            var file = System.IO.File.ReadAllBytes(PictureName);
            try
            {
                System.IO.File.Delete(PictureName);
            }
            catch
            {
            }
            return file;
        }

        /// <summary>
        /// 尝试获取视频某一秒的截图
        /// </summary>
        /// <param name="src"></param>
        /// <param name="timeoff"></param>
        /// <returns></returns>
        public static byte[] GetFrame(string src, double timeoff = 1.0)
        {
            if (!System.IO.File.Exists(FfmpegPath))
            {
                throw new Exception("Ffmpeg not found: " + FfmpegPath);
            }
            if (!System.IO.File.Exists(src))
            {
                throw new Exception("The video file is not exist: " + src);
            }
            var _timeoff = timeoff.ToString("0.000");
            System.Diagnostics.ProcessStartInfo FilestartInfo = new System.Diagnostics.ProcessStartInfo(FfmpegPath);
            var PictureName = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + ".jpg";
            FilestartInfo.Arguments = String.Format("-i \"{0}\" -ss {1} -t 0.001 -f image2 \"{2}\"", src, _timeoff, PictureName);
            var proc = System.Diagnostics.Process.Start(FilestartInfo);
            proc.WaitForExit();
            var file = System.IO.File.ReadAllBytes(PictureName);
            try
            {
                System.IO.File.Delete(PictureName);
            }
            catch
            {
            }
            return file;
        }

        /// <summary>
        /// 获取片长
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static VideoInfo GetVideoInfo(string src)
        {
            using (System.Diagnostics.Process ffmpeg = new System.Diagnostics.Process())
            {
                String duration;
                String result;
                StreamReader errorreader;
                ffmpeg.StartInfo.UseShellExecute = false;
                ffmpeg.StartInfo.RedirectStandardError = true;
                ffmpeg.StartInfo.FileName = FfmpegPath;
                ffmpeg.StartInfo.Arguments = "-i \"" + src + "\"";
                ffmpeg.Start();
                errorreader = ffmpeg.StandardError;
                ffmpeg.WaitForExit();
                result = errorreader.ReadToEnd();
                System.Diagnostics.Debug.WriteLine(result);
                var ret = new VideoInfo();
                duration = result.Substring(result.IndexOf("Duration: ") + ("Duration: ").Length, ("00:00:00").Length);
                var tmp = duration.Split(':');
                ret.Duration = new TimeSpan(int.Parse(tmp[0]), int.Parse(tmp[1]), int.Parse(tmp[2]));
                var tmp2 = result.Split('\n');
                for (var i = 0; i < tmp2.Length; i++)
                {
                    if (tmp2[i].IndexOf("Duration") >= 0)
                    {
                        var tmp3 = tmp2[i].Split(',');
                        foreach (var s in tmp3)
                        {
                            if (s.IndexOf("bitrate:") >= 0)
                            {
                                ret.Biterate = Convert.ToInt32(s.Replace("bitrate:", "").Replace("kb/s", "").Trim(' '));
                            }
                        }
                    }
                    if (tmp2[i].IndexOf("Video:") >= 0)
                    {
                        var tmp3 = tmp2[i].Split(',');
                        foreach (var s in tmp3)
                        {
                            if (s.IndexOf("SAR") >= 0)
                            {
                                var ss = s.Trim(' ');
                                ret.Width = Convert.ToInt32(ss.Split(' ')[0].Split('x')[0]);
                                ret.Height = Convert.ToInt32(ss.Split(' ')[0].Split('x')[1]);
                            }
                        }
                    }
                }
                return ret;
            }
        }

        public static VideoInfo GetImageInfo(string src)
        {
            using (System.Diagnostics.Process ffmpeg = new System.Diagnostics.Process())
            {
                String result;
                StreamReader errorreader;
                ffmpeg.StartInfo.UseShellExecute = false;
                ffmpeg.StartInfo.RedirectStandardError = true;
                ffmpeg.StartInfo.FileName = FfmpegPath;
                ffmpeg.StartInfo.Arguments = "-i \"" + src + "\"";
                ffmpeg.Start();
                errorreader = ffmpeg.StandardError;
                ffmpeg.WaitForExit();
                result = errorreader.ReadToEnd();
                System.Diagnostics.Debug.WriteLine(result);
                var ret = new VideoInfo();
                var tmp2 = result.Split('\n');
                for (var i = 0; i < tmp2.Length; i++)
                {
                    if (tmp2[i].IndexOf("Video:") >= 0)
                    {
                        var tmp3 = tmp2[i].Split(',');
                        foreach (var s in tmp3)
                        {
                            if (s.IndexOf("x") >= 0)
                            {
                                try
                                {
                                    var ss = s.Trim(' ');
                                    ret.Width = Convert.ToInt32(ss.Split(' ')[0].Split('x')[0]);
                                    ret.Height = Convert.ToInt32(ss.Split(' ')[0].Split('x')[1]);
                                }
                                catch { }
                            }
                        }
                    }
                }
                return ret;
            }
        }

        /// <summary>
        /// 拼接视频
        /// </summary>
        /// <param name="src">视频源文件列表</param>
        /// <param name="dest">目标文件</param>
        /// <returns></returns>
        public static bool Concat(List<string> src, string dest, Quality Quality)
        {
            if (!System.IO.File.Exists(FfmpegPath))
            {
                throw new Exception("Ffmpeg not found: " + FfmpegPath);
            }
            var tmp = new Queue<string>();
            foreach (var s in src)
            {
                var flag = false;
                var fname = Path.GetTempPath() + "pomelo_" + Guid.NewGuid().ToString().Replace("-", "") + ".ts";
                try
                {
                    flag = FormatConvert(s, fname, Quality);
                }
                catch
                {
                }
                if (flag)
                    tmp.Enqueue(fname);
            }
            var files = "";
            foreach (var s in tmp)
            {
                files += s + "|";
            }
            files = files.Trim('|');

            System.Diagnostics.ProcessStartInfo FilestartInfo = new System.Diagnostics.ProcessStartInfo(FfmpegPath);
            FilestartInfo.Arguments = String.Format("\"concat: {0}\" -acodec copy -vcodec copy -absf aac_adtstoasc \"{1}\"", files, dest);
            try
            {
                var proc = System.Diagnostics.Process.Start(FilestartInfo);
                proc.WaitForExit();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 截取视频
        /// </summary>
        /// <param name="src">视频源文件</param>
        /// <param name="dest">目标文件</param>
        /// <param name="Begin">开始截取时间（秒）</param>
        /// <param name="Length">截取长度</param>
        /// <returns>成功返回真，失败返回假  </returns>
        public static bool Intercept(string src, string dest, double Begin, TimeSpan Length, Quality Quality)
        {
            if (!System.IO.File.Exists(FfmpegPath))
            {
                throw new Exception("Ffmpeg not found: " + FfmpegPath);
            }
            if (!System.IO.File.Exists(src))
            {
                throw new Exception("The video file is not exist: " + src);
            }
            System.Diagnostics.ProcessStartInfo FilestartInfo = new System.Diagnostics.ProcessStartInfo(FfmpegPath);
            FilestartInfo.Arguments = String.Format("-i \"{0}\" " + Args[Convert.ToInt32(Quality)] + " -ss {1} -t {2} \"{3}\"", src, Begin, Length, dest);
            try
            {
                var proc = System.Diagnostics.Process.Start(FilestartInfo);
                proc.WaitForExit();
            }
            catch 
            {
                return false;
            }
            return true;
        }
    }
}
