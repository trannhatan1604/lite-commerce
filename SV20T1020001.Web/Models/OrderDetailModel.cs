using SV20T1020001.DomainModels;

namespace SV20T1020001.Web.Models
{
	public class OrderDetailModel
	{
		public Order Order { get; set; }
		public List<OrderDetail> Details { get; set; }
	}
}
