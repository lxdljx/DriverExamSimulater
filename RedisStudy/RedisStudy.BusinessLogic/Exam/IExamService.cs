using ABSystem.CommService.Base;
using RedisStudy.Models;
using RedisStudy.ViewModel.Models.Exam;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RedisStudy.BusinessLogic.Exam
{
    /// <summary>
    /// 考试业务逻辑服务接口
    /// </summary>
    public interface IExamService
    {
        /// <summary>
        /// 开始考试的方法
        /// </summary>
        /// <param name="model">开始考试视图模型</param>
        /// <returns>持久化结果</returns>
        Task<PersistentResult> ExamBegin(ExamBeginViewModel model);


        /// <summary>
        /// 结束考试的方法
        /// </summary>
        /// <param name="model">结束考试的方法</param>
        /// <returns>持久化结果</returns>
        Task<PersistentResult> ExamEnd(ExamEndViewModel model);

        /// <summary>
        /// 考试项目开始的方法
        /// </summary>
        /// <param name="model">考试项目开始视图模型</param>
        /// <returns>持久化结果</returns>
        Task<PersistentResult> ExamProjectBegin(ExamProjectBeginViewModel model);

        /// <summary>
        /// 考试项目结束的方法
        /// </summary>
        /// <param name="model">考试项目结束视图模型</param>
        /// <returns>持久化结果</returns>
        Task<PersistentResult> ExamProjectEnd(ExamProjectEndViewModel model);

        /// <summary>
        /// 扣分的方法
        /// </summary>
        /// <param name="model">扣分视图模型</param>
        /// <returns>持久化结果对象</returns>
        Task<PersistentResult> ExamLostPoint(ExamLostPointViewModel model);


        /// <summary>
        /// 考试结束更新结果的方法
        /// </summary>
        /// <param name="model">考试整个结束视图模型</param>
        /// <returns>持久化结果</returns>
        Task<PersistentResult> ExamAllEnd(ExamAllEndViewModel model);

        /// <summary>
        /// 查询所有已经分配车的学员的方法
        /// </summary>
        /// <returns>查询结果</returns>
        Task<QueryResults<StudentViewModel>> QueryCaredStudents();
    }
}
