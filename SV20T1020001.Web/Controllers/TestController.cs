using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020001.Web.Models;

namespace SV20T1020001.Web.Controllers
{
    public class TestController :Controller
    {
        public IActionResult Create()
        {
            var model = new Models.Person()
            {
                Name = "Trần Nhật An",
                BirthDate = new DateTime(1990,10,25),
                Salary = 10.25m
            };
            return View(model);
        }
        public IActionResult Save(Models.Person model, string BirthDateInput ="")
        {
            //Chuyển birthDateInput sang giá trị kiểu ngày
            DateTime? date = StringtoDateTime(BirthDateInput);
            if(date.HasValue)
            {
                model.BirthDate = date.Value;
            }
            return Json(model);
        }
        public DateTime? StringtoDateTime(string s, string format = "d/M/yyyy;d-M-yyyy;d.M.yyyy")
        {
            try
            {
                return DateTime.ParseExact(s,format.Split(';'),CultureInfo.InvariantCulture);
            }
            catch { return null; }
        }
    }
    
}
