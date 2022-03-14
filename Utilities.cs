using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace webDemo.Helpers
{
    public static class Utilities
    {
        public static int Page_Size = 20;

        public static string UploadFileToFolder(IFormFile file, string folderName)
        {
            try
            {
                var fileName = $"{DateTime.Now.Ticks}_{file.FileName}";
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", folderName, fileName);
                using (var myFile = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(myFile);
                }
                return fileName;
            }
            catch
            {
                return string.Empty;
            }
        }
        public static async Task<string> UploadFile(Microsoft.AspNetCore.Http.IFormFile file, string sDirectory, string newname = null)
        {
            try
            {
                var fileName = $"{DateTime.Now.Ticks}_{file.FileName}";
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", sDirectory,fileName);
                string path2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", sDirectory,fileName);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (!System.IO.Directory.Exists(path2))
                {
                    Directory.CreateDirectory(path2);
                }
                var supportedTypes = new[] { "jpg", "jpeg", "png", "gif" };
                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt.ToLower()))
                {
                    return null;
                }
                else
                {
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                       
                    }
                    return newname;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string SEOURL(string url)
        {
            url = url.ToLower();
            url = Regex.Replace(url, @"[áàạãảăắằẳẵặâấầậẫẩ]", "a");
            url = Regex.Replace(url, @"[éèẹẽẻêếềệểễ]", "e");
            url = Regex.Replace(url, @"[óòõọỏốồỗổộơờớởợỡ]", "o");
            url = Regex.Replace(url, @"[úùũủụưứửữựừ]", "u");
            url = Regex.Replace(url, @"[ìíĩỉị]", "i");
            url = Regex.Replace(url, @"[ỳỹýỷỵ]", "y");
            url = Regex.Replace(url, @"[đ]", "d");
            url = Regex.Replace(url.Trim(),@"[^0-9a-z-\s]", "").Trim();
            url = Regex.Replace(url.Trim(),@"\s+", "-");
            url = Regex.Replace(url, @"\s", "-");
            while (true)
            {
                if (url.IndexOf("--") != -1)
                {
                    url = url.Replace("--", "-");
                }
                else
                {
                    break;
                }
            }
            return url;
        }
    }
}
