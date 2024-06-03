using System;
using System.Drawing.Printing;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020001.BusinessLayers;
using SV20T1020001.DomainModels;
using SV20T1020001.Web.AppCodes;
using SV20T1020001.Web.Models;

namespace SV20T1020001.Web.Controllers
{
	[Authorize(Roles = $"{WebUserRoles.Admininistrator},{WebUserRoles.Employee}")]
	public class ProductController : Controller
    {
		private const int PAGE_SIZE = 20;
        private const string PRODUCT_SEARCH = "Product_search";
        public IActionResult Index()
        {
			ProductSearchInput? input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            if (input == null)
            {
                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CatgoryID=0,
                    SupplierID=0,
                };
            }
            return View(input);
        }

        public IActionResult Search(ProductSearchInput input)
        {
            int rowCount = 0;

            var data = ProductDataService.ListProducts(out rowCount, input.Page, PAGE_SIZE,
             input.SearchValue ?? "", input.CatgoryID, input.SupplierID
             );
            var model = new ProductSearchResult()
            {
                Data = data,
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Categories = CommonDataService.ListOfCategories(out rowCount, 1, PAGE_SIZE, ""),
                Suppliers = CommonDataService.ListOfSuppliers(out rowCount, 1, PAGE_SIZE, "")
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";
            ViewBag.IsEdit = false;
            //chỉ định view để dùng chung với edit
            Product model = new Product()
            {
                ProductID = 0,
                Photo = "nophoto.png"
            };
            return View("Edit", model);
        }
        
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin mặt hàng";
            ViewBag.IsEdit = true;
            
			Product? model = ProductDataService.GetProduct(id);
			if (model == null)
			{
				//về lại trang chủ
				return RedirectToAction("Index");
			}
            if (string.IsNullOrEmpty(model.Photo))
                model.Photo = "nophoto.png";
           
            //kiểu customer
            return View(model);
        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            var model = ProductDataService.GetProduct(id);
            if (model == null)
                return RedirectToAction("Index");

            ViewBag.AllowDelete = !ProductDataService.InUsedProduct(id);
            return View(model);
        }
        [HttpPost]
        public IActionResult Save(Product data, IFormFile? uploadPhoto)
        {
            try
            {
				if (string.IsNullOrWhiteSpace(data.ProductName))
				{
					ModelState.AddModelError(nameof(data.ProductName), "Tên không được để trống!");
				}
				if (string.IsNullOrWhiteSpace(data.Unit))
				{
					ModelState.AddModelError(nameof(data.Unit), "Đơn vị tính không được để trống!");
				}
				if (data.Price == 0)
				{
					ModelState.AddModelError(nameof(data.Price), "Giá hàng không được bằng 0!");
				}
				if (data.Price < 0)
				{
					ModelState.AddModelError(nameof(data.Price), "Giá hàng không được âm!");
				}
				if (!decimal.TryParse(data.Price.ToString(), out _))
                {
                    ModelState.AddModelError(nameof(data.Price), "Giá phải là kiểu số!");
                }
                
                if (data.CategoryID == 0)
				{
					ModelState.AddModelError(nameof(data.CategoryID), "Loại hàng không được để trống!");
				}
				if (data.SupplierID == 0)
				{
					ModelState.AddModelError(nameof(data.SupplierID), "Nhà cung cấp không được để trống!");
				}
                if (!ModelState.IsValid)
				{
					ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng" : "Cập nhật mặt hàng";
                    ViewBag.IsEdit = false;
                    return View("Edit", data);
				}
                
				if (uploadPhoto != null)
                {
                    //tránh việc trùng tên file nên thêm time trước tên
                    string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                    string folder = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath,
                        @"images\products");//đường dẫn đến thư mục
                    string filePath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        uploadPhoto.CopyTo(stream);
                    }
                    data.Photo = fileName;
                }
                if (data.ProductID == 0)
                {
                    int id = ProductDataService.AddProduct(data);
					if (id <= 0)
					{
						ViewBag.IsEdit = false;
						ModelState.AddModelError(nameof(data.ProductName), "Hàng bị trùng lặp!");
						return View("Edit", data);
					}
				}
                else
                {
                    bool result = ProductDataService.UpdateProduct(data);
					if (!result)
					{
						
						ModelState.AddModelError(nameof(data.ProductName), "Hàng bị trùng lặp!");
						ViewBag.IsEdit = true;
						return View("Edit", data);
					}
				}
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Không thể lưu được dữ liệu. Vui lòng thử lại trong vài phút!");
				ViewBag.IsEdit = true;
				return View("Edit", data);
            }
        }
        public IActionResult SavePhoto(ProductPhoto data, IFormFile? uploadPhoto)
        {
            if (uploadPhoto != null)
            {
                //tránh việc trùng tên file nên thêm time trước tên
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string folder = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\products");//đường dẫn đến thư mục
                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }
            try
            {
                if (data.Photo.Equals("nophoto.png"))
                {
                    ModelState.AddModelError(nameof(data.Photo), "Ảnh không được để trống!");
                }
				if (string.IsNullOrWhiteSpace(data.Description))
				{
					ModelState.AddModelError(nameof(data.Description), "Mô tả không được để trống!");
				}
				if (!ModelState.IsValid)
                {
                    ViewBag.Title = data.PhotoID == 0 ? "Bổ sung ảnh" : "Thay đổi Ảnh";
                    return View("Photo", data);
                }    
                if (data.PhotoID == 0)
                {
                    long id = ProductDataService.AddPhoto(data);
                    if (id<= 0)
                    {
                        ModelState.AddModelError(nameof(data.DisplayOrder), "Số thứ tự này đã tồn tại!");
                        return View("Photo", data);
                    }
                }
                else
                {
                    bool result = ProductDataService.UpdatePhoto(data);
                    if (!result)
                    {
                        ModelState.AddModelError(nameof(data.DisplayOrder), "Số thứ tự này đã tồn tại!");
                        return View("Photo", data);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Không thể lưu được dữ liệu. Vui lòng thử lại trong vài phút!");
                return View("Photo", data);
            }
            return RedirectToAction("Edit", new { id = data.ProductID });
        }
        public IActionResult SaveAttribute(ProductAttribute data)
        {
            
            ViewBag.Title = data.AttributeID == 0 ? "Bổ sung thuộc tính" : "Cập nhật thuộc tính";
            try
            {
                if (string.IsNullOrWhiteSpace(data.AttributeName))
                {
                    ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính không được để trống!");
                }
                if (string.IsNullOrWhiteSpace(data.AttributeValue))
                {
                    ModelState.AddModelError(nameof(data.AttributeValue), "Giá trị thuộc tính không được để trống!");
                }
                
                if (!ModelState.IsValid)
                {
                    
                    return View("Attribute", data);
                }
                if (data.AttributeID == 0)
                {
                    long id = ProductDataService.AddAttribute(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.DisplayOrder), "Số thứ tự này đã tồn tại!");
                        return View("Attribute", data);
                    }
                }
                else
                {
                    bool result = ProductDataService.UpdateAttribute(data);
                    if (!result)
                    {
                        ModelState.AddModelError(nameof(data.DisplayOrder), "Số thứ tự này đã tồn tại!");
                        return View("Attribute", data);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Không thể lưu được dữ liệu. Vui lòng thử lại trong vài phút!");
                return View("Attribute", data);
            }
            return RedirectToAction("Edit", new { id = data.ProductID });
        }
        public IActionResult Photo(int id, string method, int photoId = 0)
        {
            var model = new ProductPhoto();
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung Ảnh";
                    model = new ProductPhoto
                    {
                        PhotoID = 0,
                        ProductID = id,
                        Photo = "nophoto.png"
                    };

                    return View("Photo", model);
                case "edit":
                    ViewBag.Title = "Thay đổi Ảnh";
                    model = ProductDataService.GetPhoto(photoId);
                    if (model == null) return RedirectToAction("Index");
                    return View("Photo", model);
                case "delete":
                    //TODO: Xóa ảnh (xóa trực tiếp, không hỏi lại)
                    ProductDataService.DeletePhoto(photoId);
					ViewBag.IsEdit = true;
					return View("Edit", ProductDataService.GetProduct(id));
				default:
                    return RedirectToAction("Index");
            }
        }
        public IActionResult Attribute(int id, string method, int attributeId = 0)
        {
            var model = new ProductAttribute();
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính";
                    model = new ProductAttribute
                    {
                        AttributeID = 0,
                        ProductID = id,
                    };

                    return View("Attribute", model);
                case "edit":
                    ViewBag.Title = "Cập nhật thuộc tính";
                    model = ProductDataService.GetAttribute(attributeId);
                    if (model == null) return RedirectToAction("Index");
                    return View("Attribute", model);
                case "delete":
                    //TODO : Xoá thuộc tính (xoá trực tiếp)
                    ProductDataService.DeleteAttribute(attributeId);
					
					return RedirectToAction("Edit", new {id=id} );
				default:
                    return RedirectToAction("Index");
            }
        }
    }
}
