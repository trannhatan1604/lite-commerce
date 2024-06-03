using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Dapper;
using SV20T1020001.DomainModels;

namespace SV20T1020001.DataLayers.SQLServer
{
    public class ProductDAL : _BaseDAL, IProductDAL
    {
        public ProductDAL(string connectString) : base(connectString)
        {
        }

        public int Add(Product data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Products where ProductName = @ProductName)
                                select -1
                            else
                                begin
                                    insert into Products(ProductName,ProductDescription,SupplierID,CategoryID,Unit,Price,Photo,IsSelling)
                                    values(@ProductName,@ProductDescription,@SupplierID,@CategoryID,@Unit,@Price,@Photo,@IsSelling);
                                    select @@identity;
                                end";
                //identity: danh tính-id tự tạo-kh phải tham số vì @@
                //gán tham số
                var parameters = new
                {
                    ProductName = data.ProductName??"",
                    ProductDescription = data.ProductDescription??"",
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit??"",
                    Price = data.Price,
                    Photo = data.Photo??"",
                    IsSelling = data.IsSelling
                };
                //Thực thi câu lệnh ?Query, x Scalar, NonQuery
                //parameters:thông số
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddAttribute(ProductAttribute data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from ProductAttributes where ProductID = @ProductID and DisplayOrder = @DisplayOrder)
                                select -1
                            else
                                begin
                                    insert into ProductAttributes(ProductID,AttributeName,AttributeValue,DisplayOrder)
                                    values(@ProductID,@AttributeName,@AttributeValue,@DisplayOrder);
                                    select @@identity
                                end";
                //identity: danh tính-id tự tạo-kh phải tham số vì @@
                //gán tham số
                var parameters = new
                {
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName??"",
                    AttributeValue = data.AttributeValue??"",
                    DisplayOrder = data.DisplayOrder
                };
                //Thực thi câu lệnh ?Query, x Scalar, NonQuery
                //parameters:thông số
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddPhoto(ProductPhoto data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from ProductPhotos where ProductID = @ProductID and DisplayOrder = @DisplayOrder)
                                select -1
                            else
                                begin
                                    insert into ProductPhotos(ProductID,Photo,Description,DisplayOrder,IsHidden)
                                    values(@ProductID,@Photo,@Description,@DisplayOrder,@IsHidden);
                                    select @@identity
                                end";
                //identity: danh tính-id tự tạo-kh phải tham số vì @@
                //gán tham số
                var parameters = new
                {
                    ProductID = data.ProductID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden
                };
                //Thực thi câu lệnh ?Query, x Scalar, NonQuery
                //parameters:thông số
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%" + searchValue + "%";
            }
            using (var connection = OpenConnection())
            {
                string sql = @"Select COUNT(*) from Products 
                            where 
                            ( @SearchValue = N'' OR ProductName LIKE  @SearchValue )
                                    and ( @CategoryID = 0 OR CategoryID = @CategoryID )
                                    and ( @SupplierID = 0 OR SupplierId = @SupplierID )
                                    and ( (Price BETWEEN @MinPrice AND @MaxPrice) OR (@MinPrice = 0 AND @MaxPrice = 0) )";
                var parameters = new
                {
                    searchValue = searchValue??"",
                    categoryID = categoryID,
                    supplierID = supplierID,
                    minPrice = minPrice,
                    maxPrice = maxPrice
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public bool Delete(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
				var sql = @"
                            DELETE FROM ProductAttributes WHERE ProductID = @productID;
                            DELETE FROM ProductPhotos WHERE ProductID = @productID;
                            DELETE FROM Products WHERE ProductID = @productID
                        ";
				var parameters = new
                {
                    ProductID = productID,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public bool DeleteAttribute(long attributeID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductAttributes where AttributeID = @attributeID";
                var parameters = new
                {
                    AttributeID = attributeID,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public bool DeletePhoto(long photoID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductPhotos where PhotoID = @photoID";
                var parameters = new
                {
                    PhotoID = photoID,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public Product? Get(int productID)
        {
            Product? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Products where ProductId = @productId";
                var parameters = new
                {
                    ProductId = productID
                };
                data = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductAttribute? GetAttribute(long attributeID)
        {
            ProductAttribute? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductAttributes where AttributeId = @attributeId";
                var parameters = new
                {
                    AttributeId = attributeID
                };
                data = connection.QueryFirstOrDefault<ProductAttribute>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductPhoto? GetPhoto(long photoID)
        {
            ProductPhoto? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductPhotos where PhotoID = @photoID";
                var parameters = new
                {
                    PhotoID = photoID
                };
                data = connection.QueryFirstOrDefault<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from OrderDetails where ProductID = @productId)
                                select 1
                            else 
                                select 0";
                var parameters = new
                {
                    ProductID = productID,
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public IList<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            List<Product> data = new List<Product>();

            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%" + searchValue + "%";
            }
            using (var connection = OpenConnection())
            {
                var sql = @"with cte as
                (
                    select  *,
                            row_number() over(order by ProductName) as RowNumber
                    from    Products
                    where   (@SearchValue = N'' or ProductName like @SearchValue)
                        and (@CategoryID = 0 or CategoryID = @CategoryID)
                        and (@SupplierID = 0 or SupplierId = @SupplierID)
                        and (Price >= @MinPrice)
                        and (@MaxPrice <= 0 or Price <= @MaxPrice)
                )
                select * from cte
                where   (@PageSize = 0)
                    or (RowNumber between (@Page - 1)*@PageSize + 1 and @Page * @PageSize)";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue ?? "",
                    categoryID = categoryID,
                    minPrice = minPrice,
                    maxPrice = maxPrice,
                    supplierID = supplierID,
                };
                data = connection.Query<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public IList<ProductAttribute> ListAttributes(int productID)
        {
            List<ProductAttribute> data = new List<ProductAttribute>();

            using (var connection = OpenConnection())
            {
                var sql = @"select	*, row_number() over (order by AttributeName) as RowNumber
	                            from	ProductAttributes 
	                            where	ProductID = @ProductID
                            ";
                var parameters = new
                {
                    ProductID = productID
                };
                data = connection.Query<ProductAttribute>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public IList<ProductPhoto> ListPhotos(int productID)
        {
            List<ProductPhoto> data = new List<ProductPhoto>();

            using (var connection = OpenConnection())
            {
                var sql = @"select	*, row_number() over (order by Photo) as RowNumber
	                            from	ProductPhotos 
	                            where	ProductID = @ProductID
                            ";
                var parameters = new
                {
                    ProductID = productID
                };
                data = connection.Query<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Product data)
        {
			bool result = false;
			using (var connection = OpenConnection())
			{
				var sql = @"if not exists(select * from Products where ProductID<>@ProductID and ProductName = @ProductName)
                                begin
                                    update Products
                                    set ProductName=@ProductName,
                                        ProductDescription=@ProductDescription,
                                        SupplierID=@SupplierID,
                                        CategoryID=@CategoryID,
                                        Unit=@Unit,
                                        Price=@Price,
                                        Photo=@Photo,
                                        IsSelling=@IsSelling
                                        where ProductID = @ProductID
                                end";
				var parameters = new
				{
					ProductID = data.ProductID,
					ProductName = data.ProductName ?? "",
					ProductDescription = data.ProductDescription ?? "",
					SupplierId = data.SupplierID,
					CategoryId = data.CategoryID,
					Unit = data.Unit,
					Price = data.Price,
					Photo = data.Photo ?? "",
					IsSelling = data.IsSelling,
				};
				result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
				connection.Close();
			}
			return result;
		}

        public bool UpdateAttribute(ProductAttribute data)
        {
			bool result = false;
			using (var connection = OpenConnection())
			{
				var sql = @"if not exists(select * from ProductAttributes where AttributeID<>@AttributeID and ProductID = @ProductID and DisplayOrder = @DisplayOrder)
                            begin
                            update ProductAttributes
                            set ProductID=@ProductID,
                                AttributeName=@AttributeName,
                                AttributeValue=@AttributeValue,
                                DisplayOrder=@DisplayOrder
                                where AttributeID = @AttributeID
                            end";
				var parameters = new
				{
					AttributeId = data.AttributeID,
					ProductId = data.ProductID,
					AttributeName = data.AttributeName ?? "",
					AttributeValue = data.AttributeValue ?? "",
					DisplayOrder = data.DisplayOrder,
				};
				result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
				connection.Close();
			}
			return result;
		}

        public bool UpdatePhoto(ProductPhoto data)
        {
			bool result = false;
			using (var connection = OpenConnection())
			{
				var sql = @"if not exists(select * from ProductPhotos where PhotoID <> @PhotoID and ProductId=@ProductId and DisplayOrder = @DisplayOrder)
                     begin
                     update ProductPhotos
                     set ProductID=@ProductID,
                         Description=@Description,
                         DisplayOrder=@DisplayOrder,
                         IsHidden=@IsHidden,
                         Photo = @Photo
                         where PhotoID = @PhotoID
                     end";
				var parameters = new
				{
					photoID = data.PhotoID,
					productID = data.ProductID,
					photo = data.Photo,
					description = data.Description ?? "",
					displayOrder = data.DisplayOrder,
					isHidden = data.IsHidden
				};
				result = connection.Execute(sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
				connection.Close();
			}
			return result;
		}
    }
}
