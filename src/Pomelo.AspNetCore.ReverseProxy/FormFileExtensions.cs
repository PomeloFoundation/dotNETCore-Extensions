using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;


namespace Pomelo.AspNetCore.ReverseProxy.Extensions
{
    public static class FormFileExtentions
    {
        public static byte[] ReadAllBytes(this IFormFile self)
        {
            using (var reader = new BinaryReader(self.OpenReadStream()))
            {
                return reader.ReadBytes(Convert.ToInt32(self.Length));
            }
        }

        public static Task<byte[]> ReadAllBytesAsync(this IFormFile self)
        {
            return Task.Factory.StartNew<byte[]>(() => {
                using (var reader = new BinaryReader(self.OpenReadStream()))
                {
                    return reader.ReadBytes(Convert.ToInt32(self.Length));
                }
            });
        }

        public static string GetFormFieldName(this IFormFile self)
        {
            try
            {
                var tmp = self.ContentDisposition.Split(';');
                foreach (var str in tmp)
                {
                    var tmp2 = str.Trim(' ');
                    var tmp3 = tmp2.Split('=');
                    if (tmp3.Count() == 2 && tmp3[0].ToLower() == "name")
                        return tmp3[1].PopFrontMatch("\"").PopBackMatch("\"");
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static string GetFileName(this IFormFile self)
        {
            try
            {
                var tmp = self.ContentDisposition.Split(';');
                foreach (var str in tmp)
                {
                    var tmp2 = str.Trim(' ');
                    var tmp3 = tmp2.Split('=');
                    if (tmp3.Count() == 2 && tmp3[0].ToLower() == "filename")
                        return tmp3[1].PopFrontMatch("\"").PopBackMatch("\"");
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}