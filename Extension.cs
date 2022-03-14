using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace webDemo.Extension
{
    public static class Extension
    {
        public static string ToVnd(this double dongia)
        {
            return dongia.ToString("#,##0") + "đ";
        }
        public static string ToUrlFriendly(this string url)
        {
            var result = url.ToLower().Trim();
            result = Regex.Replace(result, "áàạãảăắằẳẵặâấầậẫẩ", "a");
            result = Regex.Replace(result, "éèẹẽẻêếềệểễ", "e");
            result = Regex.Replace(result, "óòõọỏốồỗổộơờớởợỡ", "o");
            result = Regex.Replace(result, "úùũủụưứửữựừ", "u");
            result = Regex.Replace(result, "ìíĩỉị", "i");
            result = Regex.Replace(result, "ỳỹýỷỵ", "y");
            result = Regex.Replace(result, "đ", "d");
            result = Regex.Replace(result, "[^a-z0-9-]", "");
            result = Regex.Replace(result, "(-)+", "-");
            return result;
        }
    }
}
