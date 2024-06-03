using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace SV20T1020001.DataLayers.SQLServer
{
    /// <summary>
    /// lớp cha cho các lớp cài đặt các phép xử lý dữ liệu trên SQL
    /// </summary>
    public abstract class _BaseDAL
    {
        protected string _connectionString = "";
        public _BaseDAL(string connectString)
        {
            _connectionString = connectString;
        }
        /// <summary>
        /// Tạo và mở kết nối đến csdl
        /// </summary>
        /// <returns></returns>
        protected SqlConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = _connectionString;
            connection.Open();
            return connection;
        }
    }
}
