using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020001.BusinessLayers;
using SV20T1020001.DomainModels;
using SV20T1020001.Web.AppCodes;
using SV20T1020001.Web.Models;

namespace SV20T1020001.Web.Controllers
{
	[Authorize(Roles = $"{WebUserRoles.Admininistrator}")]
	public class EmployeeController : Controller
	{
        private const int PAGE_SIZE = 9;
		private const string EMPLOYEE_SEARCH = "employee_search";

		public IActionResult Index()
		{
			///Lay dau vao tim kiem hien dang luu lai trong session
			PaignationSearchInput input = ApplicationContext.GetSessionData<PaignationSearchInput>(EMPLOYEE_SEARCH);
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

		public IActionResult Search(PaignationSearchInput input)
		{
			int rowCount = 0;
			var data = CommonDataService.ListOfEmployees(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
			var model = new EmployeeSearchResult()
			{
				Page = input.Page,
				PageSize = input.PageSize,
				SearchValue = input.SearchValue ?? "",
				RowCount = rowCount,
				Data = data
			};

			//Luu lai dien kien tim kiem vao trong Session tai database
			ApplicationContext.SetSessionData(EMPLOYEE_SEARCH, input);

			return View(model);
		}
		public IActionResult Create()
		{
			ViewBag.Title = "Bổ sung nhân viên";
            //chỉ định view để dùng chung với edit
            Employee model = new Employee() {
                EmployeeID = 0,
                BirthDate = new DateTime(1990,1,1),
                Photo = "nophoto.png"
            };
			return View("Edit",model);
		}
		public IActionResult Edit(int id = 0)
		{
			ViewBag.Title = "Cập nhật thông tin nhân viên";
			Employee? model = CommonDataService.GetEmployee(id);
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
        public IActionResult Save(Employee data, string birthDateInput, IFormFile? uploadPhoto)
        {
            try
            {
				if (string.IsNullOrWhiteSpace(data.FullName))
					ModelState.AddModelError(nameof(data.FullName), "Tên không được để trống");
				if (string.IsNullOrWhiteSpace(data.Email))
					ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập Email của nhân viên");
				if (string.IsNullOrWhiteSpace(data.Phone))
					ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại");//Su dung nameof de ten khop

				/*string pattern = @".*@.*\\.com$";
				if (Regex.IsMatch(data.Email, pattern))
					ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập đúng dạng Email");*/
				//Thong bao thuoc tinh IsValid cua ModelState de kiem tra xem co ton tai loi khong
				if (!ModelState.IsValid)
				{
					ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhật thông tin nhân viên ";
					return View("Edit", data);
				}

				//Xử lý ngày sinh
				DateTime? birthDate = birthDateInput.ToDateTime();
                if(birthDate.HasValue)
                    data.BirthDate = birthDate.Value;

                //Xử lý ảnh upload (nếu có ảnh upload thì lưu ảnh)
                if(uploadPhoto != null)
                {
                    //tránh việc trùng tên file nên thêm time trước tên
                    string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                    string folder = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, 
                        @"images\employees");//đường dẫn đến thư mục
                    string filePath = Path.Combine(folder, fileName);   
                    
                    using(var stream = new FileStream(filePath, FileMode.Create))
                    {
                        uploadPhoto.CopyTo(stream);
                    }
                    data.Photo = fileName;
                }
                if (data.EmployeeID == 0)
                {
                    int id = CommonDataService.AddEmployee(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Địa chỉ email bị trùng");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = CommonDataService.UpdateEmployee(data);
                    if (!result)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Địa chỉ email bị trùng với nhân viên khác");
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
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }
            Employee? model = CommonDataService.GetEmployee(id);
			if (model == null)
			{
				//về lại trang chủ
				return RedirectToAction("Index");
			}
            ViewBag.AllowDelete = !CommonDataService.IsUsedEmployee(id);
            return View(model);
        }
    }
}
