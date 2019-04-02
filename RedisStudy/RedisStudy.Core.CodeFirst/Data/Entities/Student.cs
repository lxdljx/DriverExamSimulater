using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RedisStudy.Data.Entities
{
    /// <summary>
    /// 证件类别数据实体
    /// </summary>
    [Table("DE_AuthType")]
    public class AuthType
    {
        /// <summary>
        /// 证件类别编号
        /// </summary>
        [Key]
        public int AuthTypeID { get; set; }
        
        /// <summary>
        /// 证件类别名称
        /// </summary>
        [StringLength(200)]
        public string AuthTypeName { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool InUse { get; set; }
    }

    [Table("DE_ExamProject")]
    public class ExamProject
    {
        /// <summary>
        /// 考试项目编号
        /// </summary>
        [Key]
        public long ExamProjectID { get; set; }

        /// <summary>
        /// 考试项目名称
        /// </summary>
        [StringLength(100)]
        public string ExamProjectName { get; set; }

        /// <summary>
        /// 考试项目编码
        /// </summary>
        [StringLength(100)]
        public string ExamProjectCode { get; set; }
    }

    /// <summary>
    /// 扣分项定义
    /// </summary>
    [Table("DE_LostPointDefine")]
    public class LostPointDefine
    {
        /// <summary>
        /// 考试项目编号
        /// </summary>
        [Key]
        public long LostPointDefineID { get; set; }

        /// <summary>
        /// 考试项目名称
        /// </summary>
        [StringLength(100)]
        public string Reason { get; set; }

        /// <summary>
        /// 考试项目编码
        /// </summary>
        [StringLength(100)]
        public string LostPointCode { get; set; }

        /// <summary>
        /// 扣分分值
        /// </summary>
        public int LostPoint { get; set; }
    }

    /// <summary>
    /// 考试车辆
    /// </summary>
    [Table("DE_ExamCar")]
    public class ExamCar
    {
        /// <summary>
        /// 考车编号
        /// </summary>
        [Key]
        public int ExamCarID { get; set; }

        [StringLength(20)]
        public string CarPlat { get; set; }

        /// <summary>
        /// 号牌
        /// </summary>
        [StringLength(20)]
        public string CarNumber { get; set; }

        /// <summary>
        /// 车型
        /// </summary>
        [StringLength(20)]
        public string CarStyle { get; set; }

        /// <summary>
        /// 子车型
        /// </summary>
        [StringLength(20)]
        public string SubStyle { get; set; }

    }

    /// <summary>
    /// 考生实体
    /// </summary>
    [Table("DE_Stuendt")]
    public class Student
    {
        /// <summary>
        /// 考生编号
        /// </summary>
        [Key]
        public long StudentID { get; set; }

        /// <summary>
        /// 考生姓名
        /// </summary>
        [StringLength(40)]
        public string StudentName { get; set; }

        /// <summary>
        /// 证件类型编号
        /// </summary>
        public int AuthTypeID { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        [ForeignKey("AuthTypeID")]
        public AuthType StudentAuthType{ get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        [StringLength(100)]
        public string AuthCode { get; set; }

        /// <summary>
        /// 考试车型
        /// </summary>
        public string CarStyle { get; set; }

        /// <summary>
        /// 考试子车型
        /// </summary>
        public string SubCarStyle { get; set; }

        /// <summary>
        /// 车辆编号
        /// </summary>
        public int ExamCarID { get; set; }

        /// <summary>
        /// 考车
        /// </summary>
        [ForeignKey("ExamCarID")]
        public  ExamCar Car{ get; set; }

        /// <summary>
        /// 考试结果
        /// </summary>
        public ExamResult Result { get; set; }

        /// <summary>
        /// 考试过程集合
        /// </summary>
        public List<Exam> Exams { get; set; }
    }

    /// <summary>
    /// 考试结果枚举类型
    /// </summary>
    public enum ExamResult
    {
        UnExam = 0,

        UnPassed = 1,

        Passed = 2
    }
         

    /// <summary>
    /// 考试过程
    /// </summary>
    [Table("DE_Exam")]
    public class Exam
    {

        /// <summary>
        /// 考生编号
        /// </summary>
        public long StudentID { get; set; }

        /// <summary>
        /// 考生
        /// </summary>
        [ForeignKey("StudentID")]
        public Student Belong { get; set; }

        /// <summary>
        /// 考试次数
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? ExamBegin { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? ExamEnd { get; set; }

        /// <summary>
        /// 考试项目集合
        /// </summary>
        public List<StudentExamProject> ExamProjects { get; set; }


        /// <summary>
        /// 项目外扣分集合
        /// </summary>
        public List<LostPoint> LostPoints { get; set; }

        
    }

    /// <summary>
    /// 考试项目
    /// </summary>
    [Table("DE_StudentExamProject")]
    public class StudentExamProject
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
        /// 所属考试过程
        /// </summary>
        [ForeignKey("StudentID,Order")]
        public Exam BelongExam { get; set; }



        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? ExamBegin { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? ExamEnd { get; set; }

        /// <summary>
        /// 考试项目编号
        /// </summary>
        public long ExamProjectID { get; set; }

        /// <summary>
        /// 考试项目名称
        /// </summary>
        [ForeignKey("ExamProjectID")]
        public ExamProject CurrentProject { get; set; }

        

        /// <summary>
        /// 项目扣分集合
        /// </summary>
        public List<ProjectLostPoint> LostPoints { get; set; }

    }

    /// <summary>
    /// 项目扣分
    /// </summary>
    [Table("DE_ProjectLostPoint")]
    public class ProjectLostPoint
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
        /// 考试项目编号
        /// </summary>
        public long ExamProjectID { get; set; }


        /// <summary>
        /// 所属考试项目
        /// </summary>
        [ForeignKey("StudentID,Order, ExamProjectID")]
        public StudentExamProject Project { get; set; }

        /// <summary>
        /// 扣分定义编号
        /// </summary>
        public long LostPointDefineID { get; set; }


        /// <summary>
        /// 扣分定义
        /// </summary>
        [ForeignKey("LostPointDefineID")]
        public LostPointDefine Define { get; set; }



        public DateTime HappenTime { get; set; }

    }

    /// <summary>
    /// 项目间扣分
    /// </summary>
    [Table("DE_LostPoint")]
    public class LostPoint
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
        /// 所属考试过程
        /// </summary>
        [ForeignKey("StudentID,Order")]
        public Exam BelongExam { get; set; }

        /// <summary>
        /// 扣分编号
        /// </summary>
        public long LostPointDefineID { get; set; }

        /// <summary>
        /// 扣分定义
        /// </summary>
        [ForeignKey("LostPointDefineID")]
        public LostPointDefine Define { get; set; }

        public DateTime HappenTime { get; set; }
    }

    [Table("DE_ExamOrder")]
    public class ExamOrder
    {
        /// <summary>
        /// 编号
        /// </summary>
        [Key]
        public int ExamOrderID { get; set; }

        /// <summary>
        /// 顺序号
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        public long ExamProjectID { get; set; }

        /// <summary>
        /// 考试项目
        /// </summary>
        [ForeignKey("ExamProjectID")]
        public ExamProject Project { get; set; }
    }


    public enum InfoType
    {
        ExamBegin = 0,
        
        ExamLostPoint = 1,
        ExamProjectBegin = 2,
        
        ExamProjectLostPoint =3,
        ExamProjectEnd = 4,
        ExamEnd = 5,
        ExamAllEnd = 6
    }


    public class ExamInfo
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
        /// 信息类型
        /// </summary>
        public InfoType Info { get; set; }

        /// <summary>
        /// 追加内容
        /// </summary>
        public string AddField { get; set; }

        /// <summary>
        /// 发生时间
        /// </summary>
        public DateTime HappenTime { get; set; }
    }
}
