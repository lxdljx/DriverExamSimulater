using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Common.Redis;
using Common.XML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RedisStudy.Models;
using RedisStudy.ViewModel;

namespace RedisStudy.MVC.Controllers
{
    public class HomeController : Controller
    {
        private static string connnstr = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                .Build().GetSection("RedisServer")["Url"];

        RedisHelper h = new RedisHelper(connnstr, 0);

        public async Task<IActionResult> Index()
        {
            string studentfile = Directory.GetCurrentDirectory() + "\\students.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(studentfile);
            List<StudentViewModel> students = XMLObjectConverter.DeserializeToObject<List<StudentViewModel>>(doc.OuterXml);
            foreach (var item in students)
            {
                await h.SortedSetAddAsync<StudentViewModel>(item.ExamCar, item, item.ID);
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
