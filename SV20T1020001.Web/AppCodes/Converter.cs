using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;

//cs trong appcodes thì xoá .AppCodes
namespace SV20T1020001.Web
{
    /// <summary>
    /// Chuyển chuỗi s sang giá trị kiểu DateTime(nếu không thành công
    /// thì trả về giá trị null
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// Hàm mở rộng chuỗi this string s
        /// </summary>
        /// <param name="s"></param>
        /// <param name="formats"></param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string s, string formats = "d/M/yyyy;d-M-yyyy;d.M.yyyy")
        {
            try
            {
                return DateTime.ParseExact(s, formats.Split(';'), CultureInfo.InvariantCulture);
            }
            catch { 
                return null;
            }
        }
    }
}
