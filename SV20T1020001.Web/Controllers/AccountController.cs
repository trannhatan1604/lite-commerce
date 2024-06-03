using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020001.BusinessLayers;
using SV20T1020001.Web.AppCodes;
namespace SV20T1020001.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous] //ghi đè lên [Authorize] ở mức controller và cho phép truy cập vào action method đó mà không cần xác thực
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]//kiểm tra token đưa lên
        public async Task<IActionResult> Login(string username = "", string password = "")
        {
            //tra lai username ve giao dien loagin khi nhap sai
            ViewBag.Username = username;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Tên, mật khẩu không được để trống");
                return View();
            }

            var userAccount = UserAccountService.Authorize(username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }

            ///Dang nhap thanh cong, tao du lieu de luu thong tin dang nhap
            var userData = new WebUserData()
            {
                UserId = userAccount.UserID,
                UserName = userAccount.UserName,
                DisplayName = userAccount.FullName,
                Email = userAccount.Email,
                Photo = userAccount.Photo,
                ClientIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                SessionId = HttpContext.Session.Id,
                AdditionalData = "",
                //chỉ có quyền Employee
                Roles = userAccount.RoleNames.Split(',').ToList(),

			};
            //Thiet lap phien dang nhap cho tai khoan
            await HttpContext.SignInAsync(userData.CreatePrincipal());

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public IActionResult AccessDenined()
        {
            return View();
        }
        public IActionResult User()
        {
            return View();
        }
		public IActionResult ChangePassWord(string email="",string oldPassword = "", string newPassword="")
		{
            
            ViewBag.pass = oldPassword;
            ViewBag.newpass = newPassword;
			try
			{
				//Kiểm soát đầu vào và đưa các thông báo lỗi vào trong ModelState(nếu có)
				if (string.IsNullOrWhiteSpace(oldPassword))
				{
					ModelState.AddModelError(nameof(oldPassword), "Mật khẩu không được để trống!");
				}
				if (string.IsNullOrWhiteSpace(newPassword))
				{
					ModelState.AddModelError(nameof(newPassword), "Mật khẩu không được để trống!");
				}
				if (newPassword.Equals(oldPassword))
				{
					ModelState.AddModelError(nameof(newPassword), "Mật khẩu mới không được trùng với mật khẩu cũ!");
				}

				//THông qua thuộc tính IsValid của ModelState để kiểm tra xem có tồn tại
				if (!ModelState.IsValid)
				{
					
					return View("User");
				}

				else
				{
					bool result = UserAccountService.CheckPassWord(email,oldPassword);
					if (!result)
					{
						ModelState.AddModelError(nameof(oldPassword), $"'{oldPassword}' không phải là mật khẩu không đúng!");
						return View("User");
					}
                    else
                    {
						bool result1 = UserAccountService.ChangePassword(email, oldPassword,newPassword);
						return RedirectToAction("Index", "Home");
					}
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("Error", "Không thể lưu được dữ liệu. Vui lòng thử lại trong vài phút!");
				return View("User");
			}
			
		}
	}
}
