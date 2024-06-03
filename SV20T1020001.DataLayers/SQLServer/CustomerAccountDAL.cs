using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SV20T1020001.DomainModels;

namespace SV20T1020001.DataLayers.SQLServer
{
	public class CustomerAccountDAL : _BaseDAL, IUserAccountDAL
	{
		public CustomerAccountDAL(string connectString) : base(connectString)
		{
		}

		public UserAccount? Authorize(string userName, string password)
		{
			throw new NotImplementedException();
		}

		public bool ChangePassword(string userName, string oldPassword, string newPassword)
		{
			throw new NotImplementedException();
		}

		public bool CheckPassWord(string email, string password)
		{
			throw new NotImplementedException();
		}
	}
}
