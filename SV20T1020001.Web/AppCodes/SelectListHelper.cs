using Microsoft.AspNetCore.Mvc.Rendering;
using SV20T1020001.BusinessLayers;

namespace SV20T1020001.Web
{
	public class SelectListHelper
	{
		//Danh sách tỉnh thành
		public static List<SelectListItem> Provinces()
		{
			List<SelectListItem> list = new List<SelectListItem>();
			list.Add(new SelectListItem()
			{
				Value = "",
				Text = "-- Chọn tỉnh/thành --"

			});
			foreach(var item in CommonDataService.ListOfProvinces())
			{
				list.Add(new SelectListItem()
				{
					Value = item.ProvinceName,
					Text = item.ProvinceName
				});
			}
			return list;
		}
        public static List<SelectListItem> Categories()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "0",
                Text = "-- Chọn loại hàng --"
            });
            foreach (var item in CommonDataService.ListOfCategories())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.CategoryID.ToString(),
                    Text = item.CategoryName
                });
            }
            return list;
        }

        public static List<SelectListItem> Suppliers()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "0",
                Text = "-- Chọn nhà cung cấp --"
            });
            foreach (var item in CommonDataService.ListOfSuppliers())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.SupplierID.ToString(),
                    Text = item.SupplierName
                });
            }
            return list;
        }

		public static List<SelectListItem> Shippers()
		{
			List<SelectListItem> list = new List<SelectListItem>();
			list.Add(new SelectListItem()
			{
				Value = "0",
				Text = "-- Chọn người giao hàng --"
			});
			foreach (var item in CommonDataService.ListOfShippers())
			{
				list.Add(new SelectListItem()
				{
					Value = item.ShipperID.ToString(),
					Text = item.ShipperName
				});
			}
			return list;
		}

        public static List<SelectListItem> OrderStatus()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "-- Trạng thái --"

            });
            foreach (var item in OrderDataService.ListOrderStatus())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.Status.ToString(),
                    Text = item.Description
                });
            }
            return list;
        }
    }
}
