using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SV20T1020001.DomainModels;

namespace SV20T1020001.DataLayers
{
	/// <summary>;
	/// Định nghĩa các phép xử lý dữ liệu liên quan đến tài khoản người dùng
	/// </summary>;

	public interface IUserAccountDAL
	{
		/// <summary>
		/// Xác thực tài khoản đăng nhập của người dùng.
		/// Hàm trả về thông tin tài khoản nếu xác thực thành công,
		/// ngược lại hàm trả về null
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		UserAccount? Authorize(string userName, string password);
		/// <summary>
		/// Đổi mật khẩu
		/// </summary>
		/// <param name=&quot;userName&quot;></param>
		/// <param name=&quot;oldPassword&quot></param>
		/// <param name=&quot;newPassword&quot></param>
		/// <returns></returns>
		bool ChangePassword(string userName, string oldPassword, string newPassword);
		/// <summary>
		/// kiểm tra mật khẩu
		/// </summary>
		/// <param name="email"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		bool CheckPassWord(string email, string password);

    }
}
