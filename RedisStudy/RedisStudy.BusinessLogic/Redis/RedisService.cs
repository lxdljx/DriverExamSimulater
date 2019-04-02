using ABSystem.CommService.Base;
using Common.Redis;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RedisStudy.BusinessLogic.Logger;
using RedisStudy.Models;
using RedisStudy.ViewModel.Models.Exam;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RedisStudy.BusinessLogic.Redis
{
    /// <summary>
    /// 缓存处理的业务逻辑实现
    /// </summary>
    public class RedisService : BaseService, IRedisService
    {
        private const string Key = "STUDENTS";

        public RedisService(ILog logger) : base(logger) { }

        private static string connnstr = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                .Build().GetSection("RedisServer")["Url"];
             

        /// <summary>
        /// 开始考试的方法
        /// </summary>
        /// <param name="model">开始考试视图模型</param>
        /// <returns>持久化结果</returns>
        public async Task<PersistentResult> ExamBegin(ExamBeginViewModel model)
        {
            if (model == null || model.StudentID == 0 || model.Order == 0)
            {
                return new PersistentResult(false, "传入参数不正确");
            }
            try
            {
                RedisHelper h = new RedisHelper(connnstr, 0);
                StudentViewModel st = await h.SortedSetByIDAsync<StudentViewModel>(Key, (int)model.StudentID);
                if (st == null)
                {
                    return new PersistentResult(false, $"未找到编号为{ model.StudentID}的学员的考试信息");
                }
                if (st.Exams.Any(x => x.Order == model.Order))
                {
                    return new PersistentResult(false, $"编号为{ model.StudentID}的学员已经开始第{model.Order}次考试了");
                }
                st.Exams.Add(new ExamViewModel { ExamBegin = model.BeginTime, Order = model.Order });
                if (await h.SortedSetUpdateByScoreAsync<StudentViewModel>(Key, st.ID, st))
                {
                    return new PersistentResult(true, null);
                }
                else
                {
                    return new PersistentResult(false, $"编号为{ model.StudentID}的学员更新缓存为开始第{model.Order}次考试失败");
                }

            }
            catch (Exception e)
            {
                _logger.Error(e);
                return new PersistentResult(false, $"编号为{ model.StudentID}的学员更新缓存为开始第{model.Order}次考试失败,错误消息为{e.GetBaseException().Message}");
            }

        }


        /// <summary>
        /// 结束考试的方法
        /// </summary>
        /// <param name="model">结束考试的方法</param>
        /// <returns>持久化结果</returns>
        public async Task<PersistentResult> ExamEnd(ExamEndViewModel model)
        {
            if (model == null || model.StudentID == 0 || model.Order == 0)
            {
                return new PersistentResult(false, "传入参数不正确");
            }
            try
            {
                RedisHelper h = new RedisHelper(connnstr,0);
                StudentViewModel st = await h.SortedSetByIDAsync<StudentViewModel>(Key, (int)model.StudentID);
                if (st == null)
                {
                    return new PersistentResult(false, $"未找到编号为{ model.StudentID}的学员的考试信息");
                }
                
                var exam = st.Exams.SingleOrDefault(x => x.Order == model.Order);
                if (exam == null)
                {
                    return new PersistentResult(false, $"未找到编号为{ model.StudentID}的学员第{model.Order}次考试信息");
                }
                if (exam.ExamEnd.HasValue)
                {
                    return new PersistentResult(false, $"编号为{ model.StudentID}的学员已经结束第{model.Order}次考试了");
                }
                exam.ExamEnd = model.EndTime;
                if (await h.SortedSetUpdateByScoreAsync<StudentViewModel>(Key, st.ID, st))
                {
                    return new PersistentResult(true, null);
                }
                else
                {
                    return new PersistentResult(false, $"编号为{ model.StudentID}的学员更新缓存为结束第{model.Order}次考试失败");
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return new PersistentResult(false, $"编号为{ model.StudentID}的学员更新缓存为结束始第{model.Order}次考试失败,错误消息为{e.GetBaseException().Message}");
            }
        }

        /// <summary>
        /// 考试项目开始的方法
        /// </summary>
        /// <param name="model">考试项目开始视图模型</param>
        /// <returns>持久化结果</returns>
        public async Task<PersistentResult> ExamProjectBegin(ExamProjectBeginViewModel model)
        {
            if (model == null || model.StudentID == 0 || model.Order == 0 || model.ProjectID == 0)
            {
                return new PersistentResult(false, "传入参数不正确");
            }
            try
            {
                RedisHelper h = new RedisHelper(connnstr,0);
                StudentViewModel st = await h.SortedSetByIDAsync<StudentViewModel>(Key, (int)model.StudentID);
                if (st == null)
                {
                    return new PersistentResult(false, $"未找到编号为{ model.StudentID}的学员的考试信息");
                }
                var exam = st.Exams.SingleOrDefault(x => x.Order == model.Order);
                var project = exam.ExamProjects.SingleOrDefault(x => x.ExamProjectID == model.ProjectID);
                if (project != null)
                {
                    return new PersistentResult(false, $"编号为{ model.StudentID}的学员已经开始第{model.Order}次项目{model.ProjectName}的考试了");
                }
                exam.ExamProjects.Add(new ExamProjectViewModel { ExamBegin = model.BeginTime, ExamProjectID = model.ProjectID, ExamProjectName = model.ProjectName, ProjectCode = model.ProjectCode });
                if (await h.SortedSetUpdateByScoreAsync<StudentViewModel>(Key, st.ID, st))
                {
                    return new PersistentResult(true, null);
                }
                else
                {
                    return new PersistentResult(false, $"编号为{ model.StudentID}的学员更新缓存为开始第{model.Order}次项目{model.ProjectName}的考试失败");
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return new PersistentResult(false, $"编号为{ model.StudentID}的学员更新缓存为开始第{model.Order}次项目{model.ProjectName}的考试失败,错误消息为{e.GetBaseException().Message}");
            }

        }

        /// <summary>
        /// 考试项目结束的方法
        /// </summary>
        /// <param name="model">考试项目结束视图模型</param>
        /// <returns>持久化结果</returns>
        public async Task<PersistentResult> ExamProjectEnd(ExamProjectEndViewModel model)
        {
            if (model == null || model.StudentID == 0 || model.Order == 0)
            {
                return new PersistentResult(false, "传入参数不正确");
            }
            try
            {
                RedisHelper h = new RedisHelper(connnstr,0);
                StudentViewModel st = await h.SortedSetByIDAsync<StudentViewModel>(Key, (int)model.StudentID);
                if (st == null)
                {
                    return new PersistentResult(false, $"未找到编号为{ model.StudentID}的学员的考试信息");
                }
                var exam = st.Exams.SingleOrDefault(x => x.Order == model.Order);
                if (exam == null)
                {
                    return new PersistentResult(false, $"未找到编号为{ model.StudentID}的学员第{model.Order}次考试信息");
                }
                var project = exam.ExamProjects.SingleOrDefault(x => x.ExamProjectID == model.ProjectID);
                if (project == null)
                {
                    return new PersistentResult(false, $"未找到编号为{ model.StudentID}的学员第{model.Order}次项目{model.ProjectName}的考试信息");
                }
                if (project.ExamEnd.HasValue)
                {
                    return new PersistentResult(false, $"编号为{ model.StudentID}的学员已经结束第{model.Order}次项目{model.ProjectName}的考试了");
                }
                project.ExamEnd = model.EndTime;
                if (await h.SortedSetUpdateByScoreAsync<StudentViewModel>(Key, st.ID, st))
                {
                    return new PersistentResult(true, null);
                }
                else
                {
                    return new PersistentResult(false, $"编号为{ model.StudentID}的学员更新缓存为结束第{model.Order}次项目{model.ProjectName}的考试失败");
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return new PersistentResult(false, $"编号为{ model.StudentID}的学员更新缓存为结束第{model.Order}次项目{model.ProjectName}的考试失败,错误消息为{e.GetBaseException().Message}");
            }
        }

        /// <summary>
        /// 扣分的方法
        /// </summary>
        /// <param name="model">扣分视图模型</param>
        /// <returns>持久化结果对象</returns>
        public async Task<PersistentResult> ExamLostPoint(ExamLostPointViewModel model)
        {
            if (model == null || model.StudentID == 0 || model.Order == 0)
            {
                return new PersistentResult(false, "传入参数不正确");
            }
            try
            {
                RedisHelper h = new RedisHelper(connnstr,0);
                StudentViewModel st = await h.SortedSetByIDAsync<StudentViewModel>(Key, (int)model.StudentID);
                if (st == null)
                {
                    return new PersistentResult(false, $"未找到编号为{ model.StudentID}的学员的考试信息");
                }
                var exam = st.Exams.SingleOrDefault(x => x.Order == model.Order);
                if (exam == null)
                {
                    return new PersistentResult(false, $"未找到编号为{ model.StudentID}的学员第{model.Order}次考试信息");
                }
                if (model.ProjectID.HasValue)
                {
                    var project = exam.ExamProjects.SingleOrDefault(x => x.ExamProjectID == model.ProjectID);
                    if (project == null)
                    {
                        return new PersistentResult(false, $"未找到编号为{ model.StudentID}的学员第{model.Order}次项目{model.ProjectName}的考试信息");
                    }

                    LostPointViewModel lp = new LostPointViewModel
                    {
                        LostPointProjectID = model.LostPointProjectID,
                        LostPointReson = model.Reason,
                        Point = model.Point,
                        HappenTime = model.HappenTime
                    };

                    project.LostPoints.Add(lp);
                }
                else
                {
                    LostPointViewModel lp = new LostPointViewModel
                    {
                        HappenTime = model.HappenTime,
                        LostPointProjectID = model.LostPointProjectID,
                        LostPointReson = model.Reason,
                        Point = model.Point
                    };
                    exam.LostPoints.Add(lp);
                }
                if (await h.SortedSetUpdateByScoreAsync<StudentViewModel>(Key, st.ID, st))
                {
                    return new PersistentResult(true, null);
                }
                else
                {
                    return new PersistentResult(false, $"编号为{ model.StudentID}的学员更新缓存为因为{model.Reason}扣分{model.Point}分失败");
                }


            }
            catch (Exception e)
            {
                _logger.Error(e);
                return new PersistentResult(false, e.GetBaseException().Message);
            }
        }


        /// <summary>
        /// 考试结束更新结果的方法
        /// </summary>
        /// <param name="model">考试整个结束视图模型</param>
        /// <returns>持久化结果</returns>
        public async Task<PersistentResult> ExamAllEnd(ExamAllEndViewModel model)
        {
            if (model == null || model.StudentID == 0)
            {
                return new PersistentResult(false, "传入参数不正确");
            }
            try
            {
                RedisHelper h = new RedisHelper(connnstr,0);
                StudentViewModel st = await h.SortedSetByIDAsync<StudentViewModel>(Key, (int)model.StudentID);
                if (st == null)
                {
                    return new PersistentResult(false, $"未找到编号为{ model.StudentID}的学员的考试信息");
                }
                st.Result = model.Passed ? ExamResultViewModel.Passed : ExamResultViewModel.UnPassed;
                if (await h.SortedSetUpdateByScoreAsync<StudentViewModel>(Key, st.ID, st))
                {
                    return new PersistentResult(true, null);
                }
                else
                {
                    string result = model.Passed ? "通过" : "未通过";
                    return new PersistentResult(false, $"编号为{ model.StudentID}的学员更新缓存为考试完成，结果为{result}失败");
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return new PersistentResult(false, e.GetBaseException().Message);
            }
        }


        /// <summary>
        /// 查询所有已经分配车的学员的方法
        /// </summary>
        /// <returns>查询结果</returns>
        public async Task<QueryResults<StudentViewModel>> QueryCaredStudentsByRange(long begin = 0, long end = 0)
        {
            if (end == 0)
            {
                end = 99999999999;
            }
            try
            {
                RedisHelper h = new RedisHelper(connnstr,0);
                List<StudentViewModel> sts = await h.SortedSetByScoreAsync<StudentViewModel>(Key, begin, end);
                return new QueryResults<StudentViewModel>(true, sts, null);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return new QueryResults<StudentViewModel>(false, null, e.GetBaseException().Message);
            }
        }

    }
}
