namespace SV20T1020001.Web.Models
{
    /// <summary>
    /// Đầu vào tìm kiếm dữ liệu để nhận dữ liệu dưới dạng phân trang
    /// </summary>
    public class PaignationSearchInput
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 1;
        public string SearchValue { get; set; } = "";
    }
    /// <summary>
    /// Đầu vào dử dụng tìm kiếm mặt hàng
    /// </summary>
    public class ProductSearchInput: PaignationSearchInput
    {
        public int CatgoryID { get; set; } = 0;
        public int SupplierID { get; set; } = 0;
    }
    
}
