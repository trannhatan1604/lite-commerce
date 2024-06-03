using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020001.BusinessLayers
{
    /// <summary>
    /// Khởi tạo, lưu trữ các thông tin cấu hình bởi BusinessLayer
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// CHuỗi kết thông số kết nối CSDL 
        /// Hàm này phải gọi trước khi chạy ứng dụng
        /// </summary>
        public static string ConnectionString { get; private set; } = "";
        public static void Initialize(string connectionString)
        {
            Configuration.ConnectionString = connectionString;
        }

    }
    //static class là gì? khác với class thông thường chỗ nào?
}
