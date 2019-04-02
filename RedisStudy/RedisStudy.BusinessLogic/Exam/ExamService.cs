using ABSystem.CommService.Base;
using Microsoft.EntityFrameworkCore;
using RedisStudy.BusinessLogic.Logger;
using RedisStudy.Data;
using RedisStudy.Models;
using RedisStudy.ViewModel.Models.Exam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABSystem.Tools;

namespace RedisStudy.BusinessLogic.Exam
{
    /// <summary>
    /// 考试业务量逻辑接口实现
    /// </summary>
    public class ExamService : BaseService,IExamService
    {
        public ExamService(ILog log) : base(log) { }

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
                using (ApplicationDbContext _db = new ApplicationDbContext())
                {
                    var already = await _db.Exams.SingleOrDefaultAsync(x => x.StudentID == model.StudentID && x.Order == model.Order);
                    //if (already != null)
                    if(already != null && already.ExamBegin.HasValue)
                    {
                        return new PersistentResult(false, $"编号为{ model.StudentID}的学员已经开始第{model.Order}次考试了");
                    }
                    else if (already == null)
                    {
                        Data.Entities.Exam exam = new Data.Entities.Exam
                        {
                            StudentID = model.StudentID,
                            Order = model.Order,
                            ExamBegin = model.BeginTime
                        };
                        _db.Exams.Add(exam);
                    }
                    else
                    {
                        already.ExamBegin = model.BeginTime;
                    }
                    await _db.SaveChangesAsync();
                    return new PersistentResult(true, null);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return new PersistentResult(false, e.GetBaseException().Message);
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
                using (ApplicationDbContext _db = new ApplicationDbContext())
                {
                    var already = await _db.Exams.SingleOrDefaultAsync(x => x.StudentID == model.StudentID && x.Order == model.Order);
                    if (already == null)
                    {
                        already = new Data.Entities.Exam
                        {
                            StudentID = model.StudentID,
                            Order = model.Order,
                            ExamEnd = model.EndTime
                        };
                        _db.Exams.Add(already);
                    }
                    else if(already.ExamEnd.HasValue)
                    {
                        return new PersistentResult(false, $"编号为{ model.StudentID}的学员第{model.Order}次考试已经结束");
                    }
                    
                    
                    already.ExamEnd = model.EndTime;
                    
                    await _db.SaveChangesAsync();
                    return new PersistentResult(true, null);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return new PersistentResult(false, e.GetBaseException().Message);
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
                using (ApplicationDbContext _db = new ApplicationDbContext())
                {
                    var already = await _db.StudentExamProjects.SingleOrDefaultAsync(x => x.StudentID == model.StudentID && x.Order == model.Order && x.ExamProjectID == model.ProjectID);
                    if (already != null && already.ExamBegin.HasValue)
                    {
                        return new PersistentResult(false, $"编号为{ model.StudentID}的学员已经开始第{model.Order}次{model.ProjectName}项目的考试了");
                    }
                    if (already == null)
                    {
                        Data.Entities.StudentExamProject pro = new Data.Entities.StudentExamProject
                        {
                            StudentID = model.StudentID,
                            Order = model.Order,
                            ExamBegin = model.BeginTime,
                            ExamProjectID = model.ProjectID

                        };
                        _db.StudentExamProjects.Add(pro);
                    }
                    else
                    {
                        already.ExamBegin = model.BeginTime;
                    }
                    await _db.SaveChangesAsync();
                    return new PersistentResult(true, null);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return new PersistentResult(false, e.GetBaseException().Message);
            }
        }

        /// <summary>
        /// 考试项目结束的方法
        /// </summary>
        /// <param name="model">考试项目结束视图模型</param>
        /// <returns>持久化结果</returns>
        public async Task<PersistentResult> ExamProjectEnd(ExamProjectEndViewModel model)
        {
            if (model == null || model.StudentID == 0 || model.Order == 0 || model.ProjectID == 0)
            {
                return new PersistentResult(false, "传入参数不正确");
            }
            try
            {
                using (ApplicationDbContext _db = new ApplicationDbContext())
                {
                    var already = await _db.StudentExamProjects.SingleOrDefaultAsync(x => x.StudentID == model.StudentID && x.Order == model.Order && x.ExamProjectID == model.ProjectID);
                    if (already == null)
                    {
                        already = new Data.Entities.StudentExamProject
                        {
                            StudentID = model.StudentID,
                            Order = model.Order,
                            ExamProjectID = model.ProjectID,
                            ExamEnd = model.EndTime
                        };
                        _db.StudentExamProjects.Add(already);
                    }
                    else if (already.ExamEnd.HasValue)
                    {
                        return new PersistentResult(false, $"编号为{ model.StudentID}的学员第{model.Order}次{model.ProjectName}项目的考试已经结束");
                    }
                    else
                    {
                        already.ExamEnd = model.EndTime;
                    }
                    await _db.SaveChangesAsync();
                    return new PersistentResult(true, null);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return new PersistentResult(false, e.GetBaseException().Message);
            }
        }

        /// <summary>
        /// 扣分的方法
        /// </summary>
        /// <param name="model">扣分视图模型</param>
        /// <returns>持久化结果对象</returns>
        public async Task<PersistentResult> ExamLostPoint(ExamLostPointViewModel model)
        {
            if (model == null || model.StudentID == 0 || model.Order == 0 )
            {
                return new PersistentResult(false, "传入参数不正确");
            }
            try
            {
                using (ApplicationDbContext _db = new ApplicationDbContext())
                {
                    if (model.ProjectID.HasValue)
                    {
                        Data.Entities.ProjectLostPoint lp = new Data.Entities.ProjectLostPoint
                        {
                            StudentID = model.StudentID,
                            ExamProjectID = model.ProjectID.Value,
                            LostPointDefineID = model.LostPointDefineID,
                            Order = model.Order,
                            HappenTime = model.HappenTime
                        };
                        _db.ProjectLostPoints.Add(lp);

                    }
                    else
                    {
                        Data.Entities.LostPoint lp = new Data.Entities.LostPoint
                        {
                            StudentID = model.StudentID,
                            LostPointDefineID = model.LostPointDefineID,
                            Order = model.Order,
                            HappenTime = model.HappenTime
                        };
                        _db.LostPoints.Add(lp);
                    }
                    await _db.SaveChangesAsync();
                    return new PersistentResult(true, null);
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
            if (model == null || model.StudentID == 0 )
            {
                return new PersistentResult(false, "传入参数不正确");
            }
            try
            {
                using (ApplicationDbContext _db = new ApplicationDbContext())
                {
                    var already = await _db.Students.SingleOrDefaultAsync(x => x.StudentID == model.StudentID );
                    if (already == null)
                    {
                        return new PersistentResult(false, $"未找到编号为{ model.StudentID}的学员考试的信息，无法写入考试结果");
                    }
                    else if (already.Result != Data.Entities.ExamResult.UnExam)
                    {
                        string result = already.Result.GetDisplayName();
                        return new PersistentResult(false, $"编号为{ model.StudentID}的学员考试已经结束,结果为{result}");
                    }


                    already.Result = model.Passed ? Data.Entities.ExamResult.Passed : Data.Entities.ExamResult.UnPassed;

                    await _db.SaveChangesAsync();
                    return new PersistentResult(true, null);
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
        public async Task<QueryResults<StudentViewModel>> QueryCaredStudents()
        {
            try
            {
                using (ApplicationDbContext _db = new ApplicationDbContext())
                {
                    var students = await _db.Students.Select(s => new StudentViewModel
                    {
                        AuthCode = s.AuthCode,
                        AuthType = s.AuthTypeID,
                        AuthTypeName = s.StudentAuthType.AuthTypeName,
                        CarStyle = s.CarStyle,
                        ExamCar = s.Car.CarNumber,
                        ID = s.StudentID,
                        StudentID = s.StudentID,
                        Result = (ExamResultViewModel)(int)s.Result,
                        StudentName = s.StudentName,
                        SubCarStyle = s.SubCarStyle,
                        Exams = s.Exams.Select(m => new ExamViewModel
                        {
                            Order = m.Order,
                            ExamBegin = m.ExamBegin,
                            ExamEnd = m.ExamEnd,
                            ExamProjects = m.ExamProjects.Select(n => new ExamProjectViewModel
                            {
                                ExamBegin = n.ExamBegin,
                                ExamEnd = n.ExamEnd,
                                ExamProjectID = n.ExamProjectID,
                                ExamProjectName = n.CurrentProject.ExamProjectName,
                                ProjectCode = n.CurrentProject.ExamProjectCode,
                                LostPoints = n.LostPoints.Select(b => new LostPointViewModel
                                {
                                    LostPointProjectID = b.Define.LostPointCode,
                                    LostPointReson = b.Define.Reason,
                                    Point = b.Define.LostPoint
                                }).ToList(),
                            }).ToList(),
                            LostPoints = m.LostPoints.Select(c => new LostPointViewModel
                            {
                                LostPointProjectID = c.Define.LostPointCode,
                                LostPointReson = c.Define.Reason,
                                Point = c.Define.LostPoint
                            }).ToList()
                        }).ToList(),
                    }).ToListAsync();
                    return new QueryResults<StudentViewModel>(true, students, null);
                }

            }
            catch (Exception e)
            {
                _logger.Error(e);
                return new  QueryResults<StudentViewModel>(false,null, e.GetBaseException().Message);
            }
        }
    }

    
}
