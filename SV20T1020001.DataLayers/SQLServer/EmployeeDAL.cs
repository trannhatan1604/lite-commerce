﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SV20T1020001.DomainModels;

namespace SV20T1020001.DataLayers.SQLServer
{
	public class EmployeeDAL : _BaseDAL, ICommonDAL<Employee>
	{
		public EmployeeDAL(string connectString) : base(connectString)
		{
		}

		public int Add(Employee data)
		{
			int id = 0;
			using (var connection = OpenConnection())
			{
				var sql = @"if exists(select * from Employees where Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Employees(FullName,BirthDate,Address,Phone,Email,Photo,IsWorking)
                                    values(@FullName,@BirthDate,@Address,@Phone,@Email,@Photo,@IsWorking);
                                    select @@identity;
                                end";
				//identity: danh tính-id tự tạo-kh phải tham số vì @@
				//gán tham số
				var parameters = new
				{
					FullName = data.FullName ?? "",
					BirthDate = data.BirthDate,
					Address = data.Address ?? "",
					Phone = data.Phone ?? "",
					Email = data.Email ?? "",
                    Photo = data.Photo ?? "",

                    IsWorking = data.IsWorking,
				};
				//Thực thi câu lệnh ?Query, x Scalar, NonQuery
				//parameters:thông số
				id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
				connection.Close();
			}
			return id;
		}

		public int Count(string searchValue = "")
		{
			int count = 0;
			if (!string.IsNullOrEmpty(searchValue))
				searchValue = "%" + searchValue + "%";
			using (var connection = OpenConnection())
			{
				var sql = @"select count(*) from Employees 
                    where (@searchValue = N'') or (FullName like @searchValue)";
				var parameters = new
				{
					searchValue = searchValue ?? "",
				};
				count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
				connection.Close();
			}
			return count;
		}

		public bool Delete(int id)
		{
			bool result = false;
			using (var connection = OpenConnection())
			{
				var sql = @"delete from Employees where EmployeeID = @employeeID";
				var parameters = new
				{
					EmployeeID = id,
				};
				result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
				connection.Close();
			}

			return result;
		}

		public Employee? Get(int id)
		{
			Employee? data = null;
			using (var connection = OpenConnection())
			{
				var sql = @"select * from Employees where EmployeeID = @employeeId";
				var parameters = new
				{
					EmployeeID = id
				};
				data = connection.QueryFirstOrDefault<Employee>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
				connection.Close();
			}
			return data;
		}

		public bool IsUsed(int id)
		{
			bool result = false;
			using (var connection = OpenConnection())
			{
				var sql = @"if exists(select * from Orders where EmployeeID = @employeeId)
                                select 1
                            else 
                                select 0";
				var parameters = new
				{
					EmployeeID = id,
				};
				result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
				connection.Close();
			}
			return result;
		}

		public IList<Employee> List(int page = 1, int pageSize = 0, string searchValue = "")
		{
			List<Employee> data = new List<Employee>();

			if (!string.IsNullOrEmpty(searchValue))
			{
				searchValue = "%" + searchValue + "%";
			}
			using (var connection = OpenConnection())
			{
				var sql = @"with cte as
                            (
	                            select	*, row_number() over (order by FullName) as RowNumber
	                            from	Employees 
	                            where	(@searchValue = N'') or (FullName like @searchValue)
                            )
                            select * from cte
                            where  (@pageSize = 0) 
	                            or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                            order by RowNumber";
				var parameters = new
				{
					page = page,
					pageSize = pageSize,
					searchValue = searchValue ?? ""
				};
				data = connection.Query<Employee>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
				connection.Close();
			}
			return data;
		}

		public bool Update(Employee data)
		{
			bool result = false;
			using (var connection = OpenConnection())
			{
				//nếu email chưa trùng với đứa nào có id khác mới cập nhật
				var sql = @"if not exists(select * from Employees where EmployeeID <> @employeeId and Email = @email)
                                begin
                                    update Employees 
                                    set FullName=@FullName,
										BirthDate=@birthDate,
										Address=@address,
										Phone=@phone,
										Email=@email,
										Photo=@photo,
										IsWorking=@isWorking
                                        where EmployeeID = @employeeId
                                    end";
				var parameters = new
				{
                    employeeId = data.EmployeeID,

                    FullName = data.FullName ?? "",
					BirthDate = data.BirthDate,
					Address = data.Address ?? "",
					Phone = data.Phone ?? "",
					Email = data.Email ?? "",
                    Photo = data.Photo ??"",

                    IsWorking = data.IsWorking,
				};
				result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
				connection.Close();
			}
			return result;
		}
	}
}
