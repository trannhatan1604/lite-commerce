using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.VisualBasic;
using SV20T1020001.DomainModels;

namespace SV20T1020001.DataLayers.SQLServer
{
	public class EmployeeAccountDAL : _BaseDAL, IUserAccountDAL
	{
		public EmployeeAccountDAL(string connectionString) : base(connectionString)
		{
		}
		public UserAccount? Authorize(string userName, string password)
		{
			UserAccount data;
			using (var cn = OpenConnection())
			{
                var sql = @" select EmployeeID as UserID, Email as UserName, FullName, Email, Photo, Password, RoleNames
                        from Employees where Email = @Email AND Password = @Password ";
                var parameters = new
				{
					Email = userName,
					Password = password,
				};
				data = cn.QuerySingleOrDefault <UserAccount>(sql, parameters);

				cn.Close();
			}
			return data;
		}
		public bool ChangePassword(string userName, string oldPassword, string newPassword)
		{
			bool result = false;
			using (var cn = OpenConnection())
			{
				var sql = @"update Employees
					set Password = @NewPassword
					where Email = @Email and Password = @OldPassword" ;
				var parameters = new
				{
					Email = userName,
					OldPassword = oldPassword,
					NewPassword = newPassword
				};
				result = cn.Execute(sql, parameters) > 0;
				cn.Close();
			}
			return result;
		}
        public bool CheckPassWord(string email , string password)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Employees where Email = @Email and Password = @Password)
                                select 1
                            else 
                                select 0";
                var parameters = new
                {
                    Password = password,
                    Email = email,
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }
    }
}
