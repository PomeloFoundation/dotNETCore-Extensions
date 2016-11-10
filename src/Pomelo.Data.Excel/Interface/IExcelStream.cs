using Pomelo.Data.Excel.Infrastructure;
using System.IO;

namespace Pomelo.Data.Excel
{
    public interface IExcelStream
    {
        /// <summary>
        /// 创建ExcelStream<see cref="ExcelStream"/>
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        ExcelStream Create(string path);

        // <summary>
        /// 创建Sheet
        /// </summary>
        /// <param name="name">Sheet名称</param>
        /// <returns></returns>
        SheetWithoutHDR CreateSheet(string name);

        // <summary>
        /// 创建Sheet
        /// </summary>
        /// <param name="name">Sheet名称</param>
        /// <returns></returns>
        SheetHDR CreateSheetHDR(string name);

        /// <summary>
        /// 加载Excel,返回ExcelStream
        /// </summary>
        /// <param name="stream">excel字节流</param>
        /// <returns></returns>
        ExcelStream Load(Stream stream);

        /// <summary>
        /// 加载Excel,返回ExcelStream
        /// </summary>
        /// <param name="path">excel文件路径</param>
        /// <returns></returns>
        ExcelStream Load(string path);

        /// <summary>
        /// 加载LoadSheet不包含列名
        /// </summary>
        /// <param name="name">Sheet名称</param>
        /// <returns></returns>
        SheetWithoutHDR LoadSheet(string name);

        /// <summary>
        /// 加载LoadSheet不包含列名
        /// </summary>
        /// <param name="id">Sheet名称对于的Id，从1开始</param>
        /// <returns></returns>
        SheetWithoutHDR LoadSheet(ulong id);

        /// <summary>
        /// 加载LoadSheet包含列名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        SheetHDR LoadSheetHDR(string name);

        /// <summary>
        /// 加载LoadSheet包含列名
        /// </summary>
        /// <param name="Id">Sheet名称对应的Id，从1开始</param>
        /// <returns></returns>
        SheetHDR LoadSheetHDR(ulong Id);

        /// <summary>
        /// 删除Sheet
        /// </summary>
        /// <param name="name">Sheet名称</param>
        void RemoveSheet(string name);

        /// <summary>
        /// 删除Sheet
        /// </summary>
        /// <param name="Id">Sheet名称对应的Id</param>
        void RemoveSheet(ulong Id);
    }
}
