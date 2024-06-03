using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SV20T1020001.DataLayers;
using SV20T1020001.DataLayers.SQLServer;
using SV20T1020001.DomainModels;

namespace SV20T1020001.BusinessLayers
{
    /// <summary>
    /// CUng cấp chức năng xử lý dữ liệu chung
    /// tỉnh/thành, khách hàng, nhà cung cấp, loại hàng, người giao hàng, nhân viên
    /// </summary>
    public static class CommonDataService
    {
        private static readonly ICommonDAL<Province> provinceDB;
        private static readonly ICommonDAL<Customer> customerDB;
		private static readonly ICommonDAL<Supplier> supplierDB;
		private static readonly ICommonDAL<Shipper> shipperDB;
		private static readonly ICommonDAL<Employee> employeeDB;
		private static readonly ICommonDAL<Category> categoryDB;
		/// <summary>
		/// Câu hỏi : static contructor hoạt động như thế nào?
		/// </summary>
		static CommonDataService()
        {
            string connectionString = Configuration.ConnectionString;

            provinceDB = new ProvinceDAL(connectionString);
            customerDB = new CustomerDAL(connectionString);
			supplierDB = new SupplierDAL(connectionString);
			shipperDB = new ShipperDAL(connectionString);
			employeeDB = new EmployeeDAL(connectionString);
			categoryDB = new CategoryDAL(connectionString);
		}
        /// <summary>
        /// Danh sách tỉnh/thành
        /// </summary>
        /// <returns></returns>
        public static List<Province> ListOfProvinces()
        {
            return provinceDB.List().ToList();
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách khách hàng
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static List<Customer> ListOfCustomers(out int rowCount, int page = 1, int pageSize = 0, string searchValue ="")
        {
            rowCount = customerDB.Count(searchValue);
            return customerDB.List(page, pageSize,searchValue).ToList();
        }
        public static List<Customer> ListOfCustomers(int page = 1, int pageSize = 0, string searchValue = "")
        {
            return customerDB.List(page, pageSize, searchValue).ToList();
        }
        //Câu hỏi : Tìm hiểu kiến trúc 3-layers
        public static Customer? GetCustomer(int id)
        {
            return customerDB.Get(id);
        }
        /// <summary>
        /// thêm khách hàng
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static int AddCustomer(Customer customer)
        {
            return customerDB.Add(customer);
        }
        /// <summary>
        /// cập nhật khách hàng
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static bool UpdateCustomer(Customer customer)
        {
            return customerDB.Update(customer);
        }
        /// <summary>
        /// xoá khách hàng nếu kh sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCustomer(int id)
        {
            if (customerDB.IsUsed(id))
                return false;
            return customerDB.Delete(id);
        }
        /// <summary>
        /// Kiểm tra khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedCustomer(int id)
        {
            return customerDB.IsUsed(id);
        }

		/// <summary>
		/// Tìm kiếm và lấy danh sách nhà cung cấp
		/// </summary>
		/// <param name="rowCount"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <param name="searchValue"></param>
		/// <param name=""></param>
		/// <returns></returns>
		public static List<Supplier> ListOfSuppliers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
		{
			rowCount = supplierDB.Count(searchValue);
			return supplierDB.List(page, pageSize, searchValue).ToList();
		}
        public static List<Supplier> ListOfSuppliers(int page = 1, int pageSize = 0, string searchValue = "")
        {
            return supplierDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// Lấy thông tin 1 nhà cung cấp theo mã
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Supplier? GetSupplier(int id)
		{
			return supplierDB.Get(id);
		}
		/// <summary>
		/// thêm nhà cung cấp
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static int AddSupplier(Supplier data)
		{
			return supplierDB.Add(data);
		}
		/// <summary>
		/// cập nhật nhà cung cấp
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static bool UpdateSupplier(Supplier data)
		{
			return supplierDB.Update(data);
		}
		/// <summary>
		/// xoá nhà cung cấp nếu có mã là id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool DeleteSupplier(int id)
		{
/*			if (supplierDB.IsUsed(id))
				return false;*/
			return supplierDB.Delete(id);
		}
		/// <summary>
		/// Kiểm tra nhà cung cấp
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool IsUsedSupplier(int id)
		{
			return supplierDB.IsUsed(id);
		}

		/// <summary>
		/// Tìm kiếm và lấy danh sách người giao hàng
		/// </summary>
		/// <param name="rowCount"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <param name="searchValue"></param>
		/// <param name=""></param>
		/// <returns></returns>
		public static List<Shipper> ListOfShippers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
		{
			rowCount = shipperDB.Count(searchValue);
			return shipperDB.List(page, pageSize, searchValue).ToList();
		}
		public static List<Shipper> ListOfShippers(int page = 1, int pageSize = 0, string searchValue = "")
		{
			return shipperDB.List(page, pageSize, searchValue).ToList();
		}
		/// <summary>
		/// Lấy thông tin 1 người giao hàng theo mã
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static Shipper? GetShipper(int id)
		{
			return shipperDB.Get(id);
		}
		/// <summary>
		/// thêm nhà cung cấp
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static int AddShipper(Shipper data)
		{
			return shipperDB.Add(data);
		}
		/// <summary>
		/// cập nhật nhà cung cấp
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static bool UpdateShipper(Shipper data)
		{
			return shipperDB.Update(data);
		}
		/// <summary>
		/// xoá người giao hàng nếu có mã là id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool DeleteShipper(int id)
		{
			if (shipperDB.IsUsed(id))
				return false;
			return shipperDB.Delete(id);
		}
		/// <summary>
		/// Kiểm tra người giao hàng có dữ liệu liên quan
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool IsUsedShipper(int id)
		{
			return shipperDB.IsUsed(id);
		}

		/// <summary>
		/// Tìm kiếm và lấy danh sách nhân viên
		/// </summary>
		/// <param name="rowCount"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <param name="searchValue"></param>
		/// <param name=""></param>
		/// <returns></returns>
		public static List<Employee> ListOfEmployees(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
		{
			rowCount = employeeDB.Count(searchValue);
			return employeeDB.List(page, pageSize, searchValue).ToList();
		}
		/// <summary>
		/// Lấy thông tin 1 nhân viên theo mã
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static Employee? GetEmployee(int id)
		{
			return employeeDB.Get(id);
		}
		/// <summary>
		/// thêm nhân viên
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static int AddEmployee(Employee data)
		{
			return employeeDB.Add(data);
		}
		/// <summary>
		/// cập nhật nhân viên
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static bool UpdateEmployee(Employee data)
		{
			return employeeDB.Update(data);
		}
		/// <summary>
		/// xoá nhân viên nếu có mã là id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool DeleteEmployee(int id)
		{
			if (employeeDB.IsUsed(id))
				return false;
			return employeeDB.Delete(id);
		}
		/// <summary>
		/// Kiểm tra nhân viên có mã id hiện có dữ liệu không
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool IsUsedEmployee(int id)
		{
			return employeeDB.IsUsed(id);
		}

		/// <summary>
		/// Tìm kiếm và lấy danh sách loại hàng
		/// </summary>
		/// <param name="rowCount"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <param name="searchValue"></param>
		/// <param name=""></param>
		/// <returns></returns>
		public static List<Category> ListOfCategories(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
		{
			rowCount = categoryDB.Count(searchValue);
			return categoryDB.List(page, pageSize, searchValue).ToList();
		}
        public static List<Category> ListOfCategories(int page = 1, int pageSize = 0, string searchValue = "")
        {
            return categoryDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// Lấy thông tin 1 loại hàng theo mã
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Category? GetCategory(int id)
		{
			return categoryDB.Get(id);
		}
		/// <summary>
		/// thêm loại hàng
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static int AddCategory(Category data)
		{
			return categoryDB.Add(data);
		}
		/// <summary>
		/// cập nhật loại hàng
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static bool UpdateCategory(Category data)
		{
			return categoryDB.Update(data);
		}
		/// <summary>
		/// xoá loại hàng nếu có mã là id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool DeleteCategory(int id)
		{
/*			if (categoryDB.IsUsed(id))
				return false;*/
			return categoryDB.Delete(id);
		}
		/// <summary>
		/// Kiểm tra loại hàng
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool IsUsedCategory(int id)
		{
			return categoryDB.IsUsed(id);
		}
	}
}
