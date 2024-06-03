using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020001.BusinessLayers;
using SV20T1020001.DomainModels;
using SV20T1020001.Web.AppCodes;
using SV20T1020001.Web.Models;

namespace SV20T1020001.Web.Controllers
{
	[Authorize(Roles = $"{WebUserRoles.Admininistrator},{WebUserRoles.Employee}")]
	public class SupplierController : Controller
    {
        private const int PAGE_SIZE = 10;
        private const string SUPPLIER_SEARCH = "supplier_search";

        public IActionResult Index()
        {
            // Lấy đầu vào tìm kiếm hiện đàn lưu lại trọng session
            PaignationSearchInput input = ApplicationContext.GetSessionData<PaignationSearchInput>(SUPPLIER_SEARCH);
            // trường hợp trong session chưa có điều kiện thì tạo mới điều kiện
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

        public IActionResult Search(PaignationSearchInput input)
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfSuppliers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new SupplierSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            // Lưu lại điều kiện tìm kiếm vào trong session
            ApplicationContext.SetSessionData(SUPPLIER_SEARCH, input);
            return View(model);
        }
        public IActionResult Create()
		{
			ViewBag.Title = "Bổ sung nhà cung cấp";
			//chỉ định view để dùng chung với edit
			Supplier model = new Supplier()
			{
				SupplierID = 0
			};
			return View("Edit", model);
		}
		public IActionResult Edit(int id=0)
		{
			ViewBag.Title = "Cập nhật thông tin nhà cung cấp";
			Supplier? model = CommonDataService.GetSupplier(id);
			if (model == null)
			{
				//về lại trang chủ
				return RedirectToAction("Index");
			}
			return View(model);
		}
        [HttpPost]
        public IActionResult Save(Supplier data)
        {
            try
            {
				if (string.IsNullOrWhiteSpace(data.SupplierName))
				{
					ModelState.AddModelError(nameof(data.SupplierName), "Tên không được để trống!");
				}
				if (string.IsNullOrWhiteSpace(data.Email))
				{
					ModelState.AddModelError(nameof(data.Email), "Email không được để trống!");
				}
				if (string.IsNullOrWhiteSpace(data.ContactName))
					ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống!");
				
				if (string.IsNullOrEmpty(data.Province))
				{
					ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh thành!");
				}

				if (!ModelState.IsValid)
				{
					ViewBag.Title = data.SupplierID == 0 ? "Bổ sung nhà cung cấp" : "Cập nhật nhà cung cấp";
					return View("Edit", data);
				}

				if (data.SupplierID == 0)
                {
                    int id = CommonDataService.AddSupplier(data);
					if (id <= 0)
					{
						ModelState.AddModelError(nameof(data.SupplierName), "Nhà cung cấp đã tồn tại!");
						return View("Edit", data);
					}
				}
                else
                {
                    bool result = CommonDataService.UpdateSupplier(data);
					if (!result)
					{
						ModelState.AddModelError(nameof(data.SupplierName), "Nhà cung cấp đã tồn tại!");
						return View("Edit", data);
					}
				}
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
				ModelState.AddModelError("Error", "Không thể lưu được dữ liệu. Vui lòng thử lại trong vài phút!");
				return View("Edit", data);
			}
        }
        public IActionResult Delete(int id = 0)
		{

            if (Request.Method == "POST")
            {
                CommonDataService.DeleteSupplier(id);
                return RedirectToAction("Index");
            }
            Supplier? model = CommonDataService.GetSupplier(id);
			if (model == null)
			{
				//về lại trang chủ
				return RedirectToAction("Index");
			}
            ViewBag.AllowDelete = !CommonDataService.IsUsedSupplier(id);
            return View(model);
		}
	}
}
