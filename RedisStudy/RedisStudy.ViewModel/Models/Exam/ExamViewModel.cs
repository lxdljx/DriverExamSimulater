using System;
using System.Collections.Generic;
using System.Text;

namespace RedisStudy.ViewModel.Models.Exam
{
    /// <summary>
    /// 考试按次开始视图模型
    /// </summary>
    public class ExamBeginViewModel
    {
        /// <summary>
        /// 考生编号
        /// </summary>
        public long StudentID { get; set; }

        /// <summary>
        /// 考试次数
        /// </summary>
        public int Order { get; set; }


        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }
    }


    /// <summary>
    /// 考试按次结束视图模型
    /// </summary>
    public class ExamEndViewModel
    {
        /// <summary>
        /// 考生编号
        /// </summary>
        public long StudentID { get; set; }

        /// <summary>
        /// 考试次数
        /// </summary>
        public int Order { get; set; }


        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime EndTime { get; set; }
    }


    /// <summary>
    /// 项目开始视图模型
    /// </summary>
    public class ExamProjectBeginViewModel
    {
        /// <summary>
        /// 考生编号
        /// </summary>
        public long StudentID { get; set; }

        /// <summary>
        /// 考试次数
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        public long ProjectID { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 项目代码
        /// </summary>
        public string ProjectCode { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }
    }

    /// <summary>
    /// 项目结束视图模型
    /// </summary>
    public class ExamProjectEndViewModel
    {
        /// <summary>
        /// 考生编号
        /// </summary>
        public long StudentID { get; set; }

        /// <summary>
        /// 考试次数
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        public long ProjectID { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime EndTime { get; set; }
    }


    public class ExamLostPointViewModel
    {
        /// <summary>
        /// 考生编号
        /// </summary>
        public long StudentID { get; set; }

        /// <summary>
        /// 考试次数
        /// </summary>
        public int Order { get; set; }

        public long? ProjectID { get; set; }

        public string ProjectName { get; set; }

        /// <summary>
        /// 项目代码
        /// </summary>
        public string ProjectCode { get; set; }

        public long LostPointDefineID { get; set; }

        public string LostPointProjectID { get; set; }

        public string Reason { get; set; }


        public int Point { get; set; }

        /// <summary>
        /// 发生时间
        /// </summary>
        public DateTime HappenTime { get; set; }
    }


    public class ExamAllEndViewModel
    {
        public long StudentID { get; set; }

        public bool Passed { get; set; }
    }
}
