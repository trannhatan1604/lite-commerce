using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020001.DataLayers
{
    /// <summary>
    /// Mô tả các phép xử lý dữ liệu chung
    /// </summary>
    public interface ICommonDAL<T> where T : class
    {
        /// <summary>
        /// Tìm kiếm và lấy danh sách dữ liệu dưới dạng phan trang
        /// </summary>
        /// <param name="page">Trang cần hiẻn thị</param>
        /// <param name="pageSize">Số dòng hiển thị trên mỗi trang(tham số này = 0 nếu không phân trang)</param>
        /// <param name="searchValue">Chuỗi giá trị cần tìm kiếm(nếu chuỗi rỗng thì lấy toàn bộ giá trị</param>
        /// <returns></returns>
        IList<T> List(int page = 1, int pageSize = 0, string searchValue = "");
        /// <summary>
        /// Đếm số dòng dữ liệu tìm được
        /// </summary>
        /// <param name="searchValue">giá trị cần tìm</param>
        /// <returns></returns>
        int Count(string searchValue = "");
        /// <summary>
        /// Bổ sung dữ liệu vào csdl. Hàm trả về ID của dữ liệu bổ sung
        /// Trả về giá trị 0 nếu việc bổ sung không thành công
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Add(T data);
        /// <summary>
        /// Cập nhật dữ liệu 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(T data);
        /// <summary>
        /// Xoá dữ liệu dựa trên id
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Delete(int id);
        /// <summary>
        /// Lấy một bản ghi dựa trên id(trả về null nếu dữ liệu không tồn tại)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T? Get(int id);
        /// <summary>
        /// Kiểm tra xem bản ghi dữ liệu có mã id hiện đang có được sử dụng bởi các dl khác không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool IsUsed(int id);
    }
}
