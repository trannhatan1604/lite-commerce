using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SV20T1020001.DomainModels;

namespace SV20T1020001.DataLayers.SQLServer
{
	public class SupplierDAL : _BaseDAL, ICommonDAL<Supplier>
	{
		public SupplierDAL(string connectString) : base(connectString)
		{
		}

		public int Add(Supplier data)
		{
			int id = 0;
			using (var connection = OpenConnection())
			{
				var sql = @"if exists(select * from Suppliers where SupplierName = @SupplierName)
                                select -1
                            else
								begin
									insert into Suppliers(SupplierName,ContactName,Province,Address,Phone,Email)
									values(@SupplierName,@ContactName,@Province,@Address,@Phone,@Email);
									select @@identity;
								end";
				var parameters = new
				{
					SupplierName = data.SupplierName ?? "",
					ContactName = data.ContactName ?? "",
					Province = data.Province ?? "",
					Address = data.Address ?? "",
					Phone = data.Phone ?? "",
					Email = data.Email ?? "",

				};
				//Thuc thi cau lenh
				id = connection.ExecuteScalar<int>(sql, param: parameters, commandType: System.Data.CommandType.Text);
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
				var sql = @"select count(*) from Suppliers 
                    where (@searchValue = N'') or (SupplierName like @searchValue)";
				var parameters = new
				{
					searchValue = searchValue ?? "",
				};
				count = connection.ExecuteScalar<int>(sql, param: parameters, commandType: System.Data.CommandType.Text);
				connection.Close();
			}
			return count;
		}

		public bool Delete(int id)
		{
			bool result = false;
			using (var connection = OpenConnection())
			{
				var sql = @"delete from Suppliers where SupplierId = @supplierId";
				var parameters = new
				{
					supplierId = id,
				};
				//Thuc thi cau lenh
				result = connection.Execute(sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
				connection.Close();
			}
			return result;
		}

		public Supplier? Get(int id)
		{
			Supplier? data = null;
			using (var connection = OpenConnection())
			{
				var sql = @"select * from Suppliers where SupplierId = @supplierId";
				var parameters = new
				{
					supplierId = id,
				};
				//Thuc thi cau lenh
				data = connection.QueryFirstOrDefault<Supplier>(sql, param: parameters, commandType: System.Data.CommandType.Text) ?? null;
				connection.Close();
			}
			return data;
		}

		public bool IsUsed(int id)
		{
			bool result = false;
			using (var connection = OpenConnection())
			{
				var sql = @"if exists(select * from Products where SupplierId = @supplierId)
                                select 1
                            else 
                                select 0";
				var parameters = new
				{
					supplierId = id,
				};
				//Thuc thi cau lenh
				result = connection.ExecuteScalar<bool>(sql, param: parameters, commandType: System.Data.CommandType.Text);
				connection.Close();
			}
			return result;
		}

		public IList<Supplier> List(int page = 1, int pageSize = 0, string searchValue = "")
		{
			List<Supplier> data = new List<Supplier>();
			if (!string.IsNullOrEmpty(searchValue))
			{
				searchValue = "%" + searchValue + "%";
			}
			using (var connection = OpenConnection())
			{
				var sql = @"with cte as
                            (
	                            select	*, row_number() over (order by SupplierName) as RowNumber
	                            from	Suppliers 
	                            where	(@searchValue = N'') or (SupplierName like @searchValue)
                            )
                            select * from cte
                            where  (@pageSize = 0) 
	                            or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                            order by RowNumber";
				var parameters = new
				{
					page = page,
					pageSize = pageSize,
					searchValue = searchValue
				};

				data = connection.Query<Supplier>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
				connection.Close();

			}

			return data;
		}

		public bool Update(Supplier data)
		{
			bool result = false;
			using (var connection = OpenConnection())
			{
				var sql = @"if not exists(select * from Suppliers where SupplierId <> @supplierId and SupplierName = @SupplierName)
                                begin
									update Suppliers 
                                    set SupplierName = @supplierName,
                                        ContactName = @contactName,
                                        Province = @province,
                                        Address = @address,
                                        Phone = @phone,
                                        Email = @email
                                    where SupplierId = @supplierId
								end	";
				var parameters = new
				{
					SupplierId = data.SupplierID,
					SupplierName = data.SupplierName ?? "",
					ContactName = data.ContactName ?? "",
					Province = data.Province ?? "",
					Address = data.Address ?? "",
					Phone = data.Phone ?? "",
					Email = data.Email ?? "",
				};
				result = connection.Execute(sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
				connection.Close();
			}
			return result;
		}
	}
}
