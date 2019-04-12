using Common.Redis;
using Newtonsoft.Json;
using RedisStudy.BusinessLogic.Logger;
using RedisStudy.Data.Entities;
using RedisStudy.Models;
using RedisStudy.ViewModel.EventObject;
using RedisStudy.ViewModel.Models.Exam;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedisStudy.UploadService
{
    public class CarSimuler
    {
        public string ExamCarNumber { get; set; }

        public int CarID { get; set; }

        private const string Channel = "SSS";

        private string Connectstring;

        private ILog Log { get; set; }

        private List<ExamProject> Projects { get; set; }

        private List<LostPointDefine> LostPointDefines { get; set; }

        private long beginIndex = 0;

        private int split = 5000;

        public CarSimuler(int carid, string carnumber, List<ExamProject> ps, List<LostPointDefine> ls, string cachestring, long beginindex, ILog log)
        {
            ExamCarNumber = carnumber;
            CarID = carid;
            Log = log;
            Projects = ps;
            LostPointDefines = ls;
            Connectstring = cachestring;
            beginIndex = beginindex;
        }

        public async Task Go()
        {

            RedisHelper publisher = new RedisHelper(Connectstring, 1);
            RedisHelper redisHelper = new RedisHelper(Connectstring, 0);
            StudentViewModel s = await redisHelper.SortedSetByIndexAsync<StudentViewModel>(CarID.ToString(), beginIndex);
            
            Log.Info($"取得编号为{s.StudentID}的学员,开始模拟。。。。。。");
            bool r = await Exam(1, publisher, s.StudentID);
            if (!r)
            {
                r = await Exam(2, publisher, s.StudentID);
            }

            await ExamAllEnd(s.StudentID, r, publisher);
            Log.Info($"编号为{s.StudentID}的学员,模拟考试结束");


        }

        public async Task<bool> Exam(int order, RedisHelper publisher, long studentid)
        {
            int Lost = 0;


            int projectcount = 0;
            Random r = new Random((int)DateTime.Now.Ticks);
            //发送开始考试

            await ExamBegin(order, studentid, publisher);
            Log.Info($"编号为{studentid}的学员,第{order}次考试开始");
            Thread.Sleep(split);
            while (Lost <= 20 && projectcount <= 4)
            {
                //选择一个项目
                int num = r.Next(Projects.Count);
                var p = Projects[num];
                //发送项目开始
                await ProjectBegin(order, studentid, p, publisher);
                Log.Info($"编号为{studentid}的学员,第{order}次考试项目{p.ExamProjectName}开始");
                Thread.Sleep(split);
                int needlost = r.Next(2);
                if (needlost == 1)
                {
                    int num_l = r.Next(LostPointDefines.Count);
                    var l = LostPointDefines[num_l];
                    Lost += l.LostPoint;
                    await LostPoint(order, studentid, p, l, publisher);

                    Log.Info($"编号为{studentid}的学员,第{order}次考试项目{p.ExamProjectName}被扣分{l.LostPoint},原因是{l.Reason}");
                    Thread.Sleep(split);
                }
                if (Lost <= 20)
                {
                    await ProjectEnd(order, studentid, p, publisher);
                    Log.Info($"编号为{studentid}的学员,第{order}次考试项目{p.ExamProjectName}结束");
                    projectcount++;
                    Thread.Sleep(split);
                }
                else
                {
                    await ExamEnd(order, studentid, publisher);
                    Log.Info($"编号为{studentid}的学员,第{order}次考试结束");
                    Thread.Sleep(split);
                    return false;
                }
            }
            
            await ExamEnd(order, studentid, publisher);
            Log.Info($"编号为{studentid}的学员,第{order}次考试结束");
            Thread.Sleep(split);
            return true;
        }


        public Task ExamBegin(int order, long studentid, RedisHelper publisher)
        {
            ExamBeginViewModel model = new ExamBeginViewModel
            {
                StudentID = studentid,
                Order = order,
                BeginTime = DateTime.Now
            };
            EventObject o = new EventObject
            {
                FromCar = CarID.ToString(),
                EventModelJson = JsonConvert.SerializeObject(model),
                Message = "考试开始",
                Type = EventType.ExamBegin
            };
            return publisher.PublishAsync<EventObject>(Channel, o);
        }

        public Task ExamEnd(int order, long studentid, RedisHelper publisher)
        {
            ExamEndViewModel model = new ExamEndViewModel
            {
                StudentID = studentid,
                Order = order,
                EndTime = DateTime.Now
            };
            EventObject o = new EventObject
            {
                FromCar = CarID.ToString(),
                EventModelJson = JsonConvert.SerializeObject(model),
                Message = "考试结束",
                Type = EventType.ExamEnd
            };
            return publisher.PublishAsync<EventObject>(Channel, o);
        }


        public Task ProjectBegin(int order, long studentid, ExamProject p, RedisHelper publisher)
        {
            ExamProjectBeginViewModel model = new ExamProjectBeginViewModel
            {
                StudentID = studentid,
                Order = order,
                ProjectID = p.ExamProjectID,
                ProjectCode = p.ExamProjectCode,
                ProjectName = p.ExamProjectName,
                BeginTime = DateTime.Now
            };
            EventObject o = new EventObject
            {
                FromCar = CarID.ToString(),
                EventModelJson = JsonConvert.SerializeObject(model),
                Message = "项目开始",
                Type = EventType.ProjectBegin
            };
            return publisher.PublishAsync<EventObject>(Channel, o);
        }

        public Task ProjectEnd(int order, long studentid, ExamProject p, RedisHelper publisher)
        {
            ExamProjectEndViewModel model = new ExamProjectEndViewModel
            {
                StudentID = studentid,
                Order = order,
                ProjectID = p.ExamProjectID,
                ProjectName = p.ExamProjectName,
                EndTime = DateTime.Now
            };
            EventObject o = new EventObject
            {
                FromCar = CarID.ToString(),
                EventModelJson = JsonConvert.SerializeObject(model),
                Message = "项目结束",
                Type = EventType.projectEnd
            };
            return publisher.PublishAsync<EventObject>(Channel, o);
        }

        public Task LostPoint(int order, long studentid, ExamProject p, LostPointDefine l, RedisHelper publisher)
        {
            ExamLostPointViewModel lost = new ExamLostPointViewModel
            {
                LostPointDefineID = l.LostPointDefineID,
                LostPointProjectID = l.LostPointCode,
                ProjectID = p?.ExamProjectID,
                ProjectCode = p?.ExamProjectCode,
                ProjectName = p?.ExamProjectName,
                Order = order,
                Reason = l.Reason,
                StudentID = studentid,
                Point = l.LostPoint,
                HappenTime = DateTime.Now
            };
            EventObject o = new EventObject
            {
                FromCar = CarID.ToString(),
                EventModelJson = JsonConvert.SerializeObject(lost),
                Message = "扣分",
                Type = EventType.LostPoint
            };
            return publisher.PublishAsync<EventObject>(Channel, o);
        }

        public Task ExamAllEnd(long studentid, bool passed, RedisHelper publisher)
        {
            ExamAllEndViewModel model = new ExamAllEndViewModel
            {
                Passed = passed,
                StudentID = studentid
            };
            EventObject o = new EventObject
            {
                FromCar = CarID.ToString(),
                EventModelJson = JsonConvert.SerializeObject(model),
                Message = "考试全部结束",
                Type = EventType.AllEnd
            };
            return publisher.PublishAsync<EventObject>(Channel, o);
        }
    }
}
