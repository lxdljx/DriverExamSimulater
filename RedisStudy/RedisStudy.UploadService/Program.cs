using Common;
using Common.XML;
using Microsoft.Extensions.Configuration;
using RedisStudy.BusinessLogic.Logger;
using RedisStudy.Data.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace RedisStudy.UploadService
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectstr = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                .Build().GetSection("RedisServer")["Url"];
            Console.WriteLine("数据发布端启动......");
            ILog log = new ConsoleLog();
            //long studentid = 1981;
            //int order = 1;
            //ExamBeginViewModel model = new ExamBeginViewModel
            //{
            //    StudentID = studentid,
            //    Order = order,
            //    BeginTime = DateTime.Now
            //};
            //string jsonstr = JsonConvert.SerializeObject(model);
            //EventObject o = new EventObject
            //{
            //    EventModelJson = jsonstr,
            //    Message = "考试开始",
            //    Type = EventType.ExamBegin
            //};
            //RedisHelper redisHelper = new RedisHelper(connectstr, 1);
            //redisHelper.Publish("SSS", JsonConvert.SerializeObject(o));
            //Console.WriteLine("数据发布开始......");
            string projectfile = Directory.GetCurrentDirectory() + "\\projects.xml";
            string lostpointfile = Directory.GetCurrentDirectory() + "\\lostpoint.xml";
            XmlDocument doc = new XmlDocument();

            doc.Load(projectfile);
            List<ExamProject> projets = XMLObjectConverter.DeserializeToObject<List<ExamProject>>(doc.OuterXml);


            XmlDocument doc1 = new XmlDocument();

            doc1.Load(lostpointfile);
            List<LostPointDefine> Losts = XMLObjectConverter.DeserializeToObject<List<LostPointDefine>>(doc1.OuterXml);


            string sinex = Console.ReadLine();
            int index = int.Parse(sinex);
            CarSimuler car = new CarSimuler(1, "111", projets, Losts, connectstr, index, log);
            Task t = car.Go();
            t.Wait();
            Console.Read();
        }


    }


}
