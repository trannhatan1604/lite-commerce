using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SV20T1020001.BusinessLayers;
using SV20T1020001.DomainModels;
using SV20T1020001.Web.AppCodes;
using SV20T1020001.Web.Models;

namespace SV20T1020001.Web.Controllers
{
	/// <summary>
	/// trong Roles cho phép các quyền
	/// $"{WebUserRoles.Customer}" : kh đc truy cập Customer
	/// </summary>
	[Authorize(Roles = $"{WebUserRoles.Admininistrator},{WebUserRoles.Employee}")]
	public class CustomerController : Controller
		
	{
        private const int PAGE_SIZE = 20;
		private const string CUSTOMER_SEARCH = "Customer_search";

        public IActionResult Index()
        {
            ///Lay dau vao tim kiem hien dang luu lai trong session
            PaignationSearchInput input = ApplicationContext.GetSessionData<PaignationSearchInput>(CUSTOMER_SEARCH);
            ///truong hop trong session chua co dieu kien thi tao dieu kien moi
            if (input == null)
            {
                input = new PaignationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }

            return View(input);
        }
        /// <summary>
        /// View thuc hien tim kiem ma ko load lai trang
        /// </summary>
        /// <returns></returns>
        public IActionResult Search(PaignationSearchInput input)
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfCustomers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new CustomerSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            //Luu lai dien kien tim kiem vao trong Session tai database
            ApplicationContext.SetSessionData(CUSTOMER_SEARCH, input);

            return View(model);
        }

        public IActionResult Create() {
			ViewBag.Title = "Bổ sung khách hàng";
			//chỉ định view để dùng chung với edit
			Customer model = new Customer() {
				CustomerId = 0
			};
			return View("Edit", model);
		}
		public IActionResult Edit(int id = 0) {

			ViewBag.Title = "Cập nhật thông tin khách hàng";
			Customer? model = CommonDataService.GetCustomer(id);
			if(model == null)
			{
				//về lại trang chủ
				return RedirectToAction("Index");
			}
			//kiểu customer
			return View(model);
		}
		
		[HttpPost]
		public IActionResult Save(Customer data)
		{
			try
			{
				//Kiểm soát đầu vào và đưa các thông báo lỗi vào trong ModelState(nếu có)
				if(string.IsNullOrWhiteSpace(data.CustomerName))
				{
					ModelState.AddModelError(nameof(data.CustomerName), "Tên không được để trống!");
				}
				if (string.IsNullOrWhiteSpace(data.ContactName))
					ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống!");
				if(string.IsNullOrWhiteSpace(data.Email))
				{
					ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập Email của khách hàng!");
				}
				if (string.IsNullOrEmpty(data.Province))
				{
					ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh thành!");
				}


				//THông qua thuộc tính IsValid của ModelState để kiểm tra xem có tồn tại
				if (!ModelState.IsValid)
				{
					ViewBag.Title = data.CustomerId == 0 ? "Bổ sung khách Hàng" : "Cập nhật khách hàng";
					return View("Edit",data);
				}

				if(data.CustomerId == 0)
				{
					int id = CommonDataService.AddCustomer(data);
					if(id <= 0)
					{
						ModelState.AddModelError(nameof(data.Email),"Địa chi email bị trùng lặp!");
						return View("Edit",data);
					}
				}
				else
				{
					bool result = CommonDataService.UpdateCustomer(data);	
					if(!result)
					{
						ModelState.AddModelError(nameof(data.Email),"Địa chỉ email bị trùng lặp!");
						return View("Edit",data);
					}
				}
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("Error", "Không thể lưu được dữ liệu. Vui lòng thử lại trong vài phút!");
				return View("Edit",data);
			}
		}
		public IActionResult Delete(int id = 0)
		{
			if (Request.Method == "POST")
			{
				CommonDataService.DeleteCustomer(id);
				return RedirectToAction("Index");
			}
			var model = CommonDataService.GetCustomer(id);
			if (model == null)
				return RedirectToAction("Index");

			ViewBag.AllowDelete = !CommonDataService.IsUsedCustomer(id);
			return View(model);
		}
	}
}
