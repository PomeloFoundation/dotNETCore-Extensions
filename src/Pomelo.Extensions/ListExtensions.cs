using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace System
{
    public static class ListExtensions
    {
        /// <summary>
        /// 将CodeComb文件组整体转移
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="DestinationDirectory">目标路径，请在结尾附带斜杠</param>
        public static void MoveTo<T>(this List<T> self, string DestinationDirectory)
            where T : MediaFile
        {
            var type = typeof(T);
            var pi = type.GetTypeInfo().GetProperty("Source");
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            var pi2 = typeof(MediaFile).GetTypeInfo().GetProperty("_Source", flag);
            foreach (var x in self)
            {
                var src = (string)pi.GetValue(x);
                var fname = DestinationDirectory + System.IO.Path.GetFileName(src);
                System.IO.File.Copy(src, fname);
                System.IO.File.Delete(src);
                pi2.SetValue(x, fname);
            };
        }

        /// <summary>
        /// 复制文件组，并成为新的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="DestinationDirectory">目标路径，请在结尾附带斜杠</param>
        public static void CopyToBe<T>(this List<T> self, string DestinationDirectory)
            where T : MediaFile
        {
            var type = typeof(T);
            var pi = type.GetTypeInfo().GetProperty("Source");
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            var pi2 = typeof(MediaFile).GetTypeInfo().GetProperty("_Source", flag);
            foreach(var x in self)
            {
                var src = (string)pi.GetValue(x);
                var fname = DestinationDirectory + System.IO.Path.GetFileName(src);
                System.IO.File.Copy(src, fname);
                pi2.SetValue(x, fname);
            }
        }

        /// <summary>
        /// 复制文件组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="DestinationDirectory">目标路径，请在结尾附带斜杠</param>
        public static void CopyTo<T>(this List<T> self, string DestinationDirectory)
            where T : MediaFile
        {
            var type = typeof(T);
            var pi = type.GetTypeInfo().GetProperty("Source");
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            var pi2 = typeof(MediaFile).GetTypeInfo().GetProperty("_Source", flag);
            foreach(var x in self)
            {
                var src = (string)pi.GetValue(x);
                var fname = DestinationDirectory + System.IO.Path.GetFileName(src);
                System.IO.File.Copy(src, fname);
            }
        }
    }
}
