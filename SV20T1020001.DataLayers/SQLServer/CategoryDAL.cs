﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SV20T1020001.DomainModels;

namespace SV20T1020001.DataLayers.SQLServer
{
	public class CategoryDAL : _BaseDAL, ICommonDAL<Category>
	{
		public CategoryDAL(string connectString) : base(connectString)
		{
		}

		public int Add(Category data)
		{
			int id = 0;
			using (var connection = OpenConnection())
			{
				var sql = @"if exists(select * from Categories where CategoryName = @CategoryName)
                                select -1
                            else 
                                begin
                                    insert into Categories(CategoryName, Description,Photo)
                                    values(@CategoryName, @Description, @Photo)
                                    select @@identity;
                                end";

				var parameters = new
				{
					CategoryName = data.CategoryName ?? "",
					Description = data.Description ?? "",
					Photo = data.Photo ?? ""
				};
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
				var sql = @"select count(*) from Categories
                            where (@searchValue = N'') or (CategoryName like @searchValue)";
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
				var sql = @"delete from Categories where CategoryId = @categoryId";
				var parameters = new
				{
					CategoryID = id,
				};
				result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
				connection.Close();
			}

			return result;
		}

		public Category? Get(int id)
		{
			Category? data = null;
			using (var connection = OpenConnection())
			{
				var sql = @"select * from Categories where CategoryId = @categoryId";
				var parameters = new
				{
					CategoryId = id,
				};
				data = connection.QueryFirstOrDefault<Category>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
				connection.Close();
			}
			return data;
		}

		public bool IsUsed(int id)
		{
			bool result = false;
			using (var connection = OpenConnection())
			{
				var sql = @"if exists(select * from Products where CategoryId = @categoryId)
                                select 1
                            else
                                select 0";
				var parameters = new
				{
					CategoryId = id,
				};
				result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
				connection.Close();

			}
			return result;
		}

		public IList<Category> List(int page = 1, int pageSize = 0, string searchValue = "")
		{
			List<Category> data = new List<Category>();
			if (!string.IsNullOrEmpty(searchValue))
				searchValue = "%" + searchValue + "%";

			using (var connection = OpenConnection())
			{
				var sql = @"with cate as
                            (
	                            select	*, row_number() over (order by CategoryName) as RowNumber
	                            from	Categories
	                            where	(@searchValue = N'') or (CategoryName like @searchValue)
                            )
                            select * from cate
                            where  (@pageSize = 0) 
	                            or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                            order by RowNumber";
				var parameters = new
				{
					page = page,
					pageSize = pageSize,
					searchValue = searchValue ?? ""
				};
				data = connection.Query<Category>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
				connection.Close();
			}

			return data;
		}

		public bool Update(Category data)
		{
			bool result = false;
			using (var connection = OpenConnection())
			{
				var sql = @"if not exists(select * from Categories where CategoryId <> @CategoryId and CategoryName = @CategoryName)
                                begin
                                    update Categories
                                    set CategoryName = @categoryName,
                                        Description = @description,
										Photo = @photo
                                    where CategoryId = @CategoryId
								end
                                ";
				var parameters = new
				{
					CategoryId = data.CategoryID,
					CategoryName = data.CategoryName ?? "",
					Description = data.Description ?? "",
					Photo = data.Photo??"",
				};
				result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
				connection.Close();
			}

			return result;
		}
	}
}
