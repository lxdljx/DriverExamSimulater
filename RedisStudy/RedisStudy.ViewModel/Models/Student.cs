using Common.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RedisStudy.Models
{
    public enum ExamResultViewModel
    {
        /// <summary>
        /// 未出结果
        /// </summary>
        [Display(Name= "未出结果")]
        UnExam = 0,

        /// <summary>
        /// 未通过
        /// </summary>
        [Display(Name= "未通过")]
        UnPassed = 1,

        /// <summary>
        /// 通过
        /// </summary>
        [Display(Name= "通过")]
        Passed = 2
    }
   

    [Serializable]
    public class StudentViewModel :BaseEntity
    {
        public long StudentID { get; set; }

        public string StudentName { get; set; }

        public int AuthType { get; set; }

        public string AuthTypeName { get; set; }

        public string AuthCode { get; set; }

        public string CarStyle { get; set; }

        public string SubCarStyle { get; set; }

        public string ExamCar { get; set; }

        

        public ExamResultViewModel Result { get; set; }

        public List<ExamViewModel> Exams { get; set; } = new List<ExamViewModel>();
    }

    [Serializable]
    public class ExamViewModel
    {
        public int Order { get; set; }

        public DateTime? ExamBegin { get; set; }

        public DateTime? ExamEnd { get; set; }

        public List<ExamProjectViewModel> ExamProjects { get; set; }

        public List<LostPointViewModel> LostPoints { get; set; }

        public ExamViewModel()
        {
            ExamProjects = new List<ExamProjectViewModel>();
            LostPoints = new List<LostPointViewModel>();
        }

        public ExamViewModel(int order):this()
        {
            Order = order;

        }
    }

    [Serializable]
    public class ExamProjectViewModel
    {
        public DateTime? ExamBegin { get; set; }

        public DateTime? ExamEnd { get; set; }

        public long ExamProjectID { get; set; }

        public string ExamProjectName { get; set; }

        public string ProjectCode { get; set; }

        public List<LostPointViewModel> LostPoints { get;set; }

        public ExamProjectViewModel() {
            LostPoints = new List<LostPointViewModel>();
        }

        public ExamProjectViewModel(int id, string pname, string code):this()
        {
            ExamProjectID = id;
            ExamProjectName = pname;
            ProjectCode = code;
            
        }

    }

    [Serializable]
    public class LostPointViewModel
    {
        public string LostPointProjectID { get; set; }

        public string LostPointReson { get; set; }

        public int Point { get; set; }

        public DateTime HappenTime { get; set; }

        public LostPointViewModel()
        { }

        public LostPointViewModel(string id, string reason, int point)
        {
            LostPointProjectID = id;
            LostPointReson = reason;
            Point = point;
        }
    }


    


}
