using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020001.BusinessLayers;
using SV20T1020001.DomainModels;
using SV20T1020001.Web.AppCodes;
using SV20T1020001.Web.Models;

namespace SV20T1020001.Web.Controllers
{
	[Authorize(Roles = $"{WebUserRoles.Admininistrator},{WebUserRoles.Employee}")]
	public class ShipperController : Controller
    {
        private const int PAGE_SIZE = 10;
        private const string SHIPPER_SEARCH = "Shipper_search";
        public IActionResult Index()
        {
            PaignationSearchInput input = ApplicationContext.GetSessionData<PaignationSearchInput>(SHIPPER_SEARCH);
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
            var data = CommonDataService.ListOfShippers(out rowCount,input.Page,input.PageSize,input.SearchValue??"");
            var model = new ShipperSearchResult()
            {
                Data = data,
                Page = input.Page,
                PageSize=input.PageSize,
                SearchValue = input.SearchValue??"",
                RowCount=rowCount,
            };
            ApplicationContext.SetSessionData(SHIPPER_SEARCH, input);
            return View(model);
        }
		public IActionResult Create()
		{
			ViewBag.Title = "Bổ sung người giao hàng";
            //chỉ định view để dùng chung với edit
            Shipper model = new Shipper() {
                ShipperID = 0
            };
			return View("Edit", model);
		}
		public IActionResult Edit(int id = 0)
		{
			ViewBag.Title = "Cập nhật thông tin người giao hàng";
			Shipper? model = CommonDataService.GetShipper(id);
			if (model == null)
			{
				//về lại trang chủ
				return RedirectToAction("Index");
			}
			return View(model);
		}
        [HttpPost]
        public IActionResult Save(Shipper data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.ShipperName))
                {
                    ModelState.AddModelError(nameof(data.ShipperName), "Tên không được để trống!");
                }
                if(string.IsNullOrWhiteSpace(data.Phone))
                {
                    ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số được thoại!");
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.Title = data.ShipperID == 0 ? "Bổ sung giao hàng" : "Cập nhật giao hàng";
                    return View("Edit", data);
                }

                if (data.ShipperID == 0)
                {
                    int id = CommonDataService.AddShipper(data);
					if (id <= 0)
					{
						ModelState.AddModelError(nameof(data.Phone), "SĐT bị trùng lặp!");
						return View("Edit", data);
					}
				}
                else
                {
                    bool result = CommonDataService.UpdateShipper(data);
					if (!result)
					{
						ModelState.AddModelError(nameof(data.Phone), "SĐT bị trùng lặp!");
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
                CommonDataService.DeleteShipper(id);
                return RedirectToAction("Index");
            }
            Shipper? model = CommonDataService.GetShipper(id);
			if (model == null)
			{
				//về lại trang chủ
				return RedirectToAction("Index");
			}
			ViewBag.AllowDelete = !CommonDataService.IsUsedShipper(id);
			return View(model);
		}
	}
}
