using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020001.BusinessLayers;
using SV20T1020001.DomainModels;
using SV20T1020001.Web.AppCodes;
using SV20T1020001.Web.Models;

namespace SV20T1020001.Web.Controllers
{
    // tương tự "employee,admin" nh dễ viết nhầm, ctrl R mới đổi tên
    [Authorize(Roles = $"{WebUserRoles.Admininistrator},{WebUserRoles.Employee}")]
    public class CategoryController : Controller
    {
		private const int PAGE_SIZE = 10;
        private const string CATEGORY_SEARCH = "category_search";
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaignationSearchInput>(CATEGORY_SEARCH);
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
            var data = CommonDataService.ListOfCategories(out rowCount, input.Page, input.PageSize, input.SearchValue);
            var model = new CategorySearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(CATEGORY_SEARCH, input);
            return View(model);
        }
        public IActionResult Create()
		{
			ViewBag.Title = "Bổ sung loại hàng hoá";
            Category model = new Category()
            {
                CategoryID = 0,
                Photo = "nophoto.png"
			};
            return View("Edit", model);
        }
		public IActionResult Edit(int id = 0)
		{
			ViewBag.Title = "Cập nhật thông tin loại hàng hoá";
			Category? model = CommonDataService.GetCategory(id);
			if (model == null)
			{
				//về lại trang chủ
				return RedirectToAction("Index");
			}
			if (string.IsNullOrEmpty(model.Photo))
				model.Photo = "nophoto.png";
			return View(model);
		}

        [HttpPost]
        public IActionResult Save(Category data, IFormFile? uploadPhoto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.CategoryName))
                    ModelState.AddModelError("CategoryName", "Tên không được để trống");
                if (string.IsNullOrWhiteSpace(data.Description))
                    ModelState.AddModelError("Description", "Mô tả không được để trống");
                if (!ModelState.IsValid)
                {
                    ViewBag.Title = data.CategoryID == 0 ? "Bổ sung loại hàng" : "Cập nhật thông tin loại hàng ";
                    return View("Edit", data);
                }

				//Xử lý ảnh upload (nếu có ảnh upload thì lưu ảnh)
				if (uploadPhoto != null)
				{
					//tránh việc trùng tên file nên thêm time trước tên
					string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
					string folder = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath,
						@"images\categories");//đường dẫn đến thư mục
					string filePath = Path.Combine(folder, fileName);

					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						uploadPhoto.CopyTo(stream);
					}
					data.Photo = fileName;
				}

				if (data.CategoryID == 0)
                {
                    int id = CommonDataService.AddCategory(data);
					if (id <= 0)
					{
						ModelState.AddModelError(nameof(data.CategoryName), "Loại hàng bị trùng lặp!");
						return View("Edit", data);
					}
				}
                else
                {
                    bool result = CommonDataService.UpdateCategory(data);
					if (!result)
					{
						ModelState.AddModelError(nameof(data.CategoryName), "Loại hàng bị trùng lặp!");
						return View("Edit", data);
					}
				}
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Không thể lưu được dữ liệu");
                return View("Edit", data);

            }
        }
        public IActionResult Delete(int id = 0)
		{

            if (Request.Method == "POST")
            {
                CommonDataService.DeleteCategory(id);
                return RedirectToAction("Index");
            }
            Category? model = CommonDataService.GetCategory(id);
			if (model == null)
			{
				//về lại trang chủ
				return RedirectToAction("Index");
			}
			ViewBag.AllowDelete = !CommonDataService.IsUsedCategory(id);
			//kiểu customer
			return View(model);
		}
	}
}
