using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedisStudy.Data.Entities;

namespace RedisStudy.Data
{
    public class ApplicationDbContext : DbContext
    {
        private static readonly IServiceProvider _serviceProvider = new ServiceCollection().AddEntityFrameworkMySql().BuildServiceProvider();

        public ApplicationDbContext()
        { }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //从配置文件加载配置信息
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            //从配置文件源构造键值对的配置信息，得到IConfigurationRoot（继承自IConfiguration）的一个实例
            IConfigurationRoot configRoot = builder.Build();
            string  connectionString = configRoot.GetSection("ConnectionStrings")["DefaultConnection"];
            optionsBuilder.UseInternalServiceProvider(_serviceProvider).UseMySql(connectionString);
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Shorten key length for Identity
            builder.Entity<IdentityUser<string>>(entity => entity.Property(m => m.Id).HasMaxLength(127));
            builder.Entity<IdentityRole<string>>(entity => entity.Property(m => m.Id).HasMaxLength(127));
            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.Property(m => m.LoginProvider).HasMaxLength(127);
                entity.Property(m => m.ProviderKey).HasMaxLength(127);
                entity.HasKey(m => m.UserId);
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.Property(m => m.UserId).HasMaxLength(127);
                entity.Property(m => m.RoleId).HasMaxLength(127);
                entity.HasKey(m => new { m.UserId, m.RoleId });
            });
            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.Property(m => m.UserId).HasMaxLength(127);
                entity.Property(m => m.LoginProvider).HasMaxLength(127);
                entity.Property(m => m.Name).HasMaxLength(127);
                entity.HasKey(m => m.UserId);
            });
            builder.Entity<Exam>(entity=> {
                entity.HasKey(m => new { m.StudentID, m.Order });
            });
            builder.Entity<StudentExamProject>(entity => {
                entity.HasKey(m => new { m.StudentID, m.Order, m.ExamProjectID });
            });
            builder.Entity<ProjectLostPoint>(entity=> {
                entity.HasKey(m => new { m.StudentID, m.Order, m.ExamProjectID, m.LostPointDefineID });
            });
            builder.Entity<LostPoint>(entity => {
                entity.HasKey(m => new { m.StudentID, m.Order,  m.LostPointDefineID });
            });
        }


        /// <summary>
        /// 证件类型数据上下文
        /// </summary>
        public DbSet<AuthType> AuthTypes { get; set; }

        /// <summary>
        /// 考车信息数据上下文
        /// </summary>
        public DbSet<ExamCar> ExamCars { get; set; }

        /// <summary>
        /// 考试项目数据上下文
        /// </summary>
        public DbSet<ExamProject> ExamProjects { get; set; }

        /// <summary>
        /// 考生信息数据上下文
        /// </summary>
        public DbSet<Student> Students { get; set; }

        /// <summary>
        /// 考试过程数据上下文
        /// </summary>
        public DbSet<Exam> Exams { get; set; }

        /// <summary>
        /// 考试项目定义数据上下文
        /// </summary>
        public DbSet<ExamOrder> ExamOrders { get; set; }

        /// <summary>
        /// 考生项目过程数据上下文
        /// </summary>
        public DbSet<StudentExamProject> StudentExamProjects { get; set; }

        /// <summary>
        /// 项目扣分数据上下文
        /// </summary>
        public DbSet<ProjectLostPoint> ProjectLostPoints { get; set; }

        /// <summary>
        /// 项目外扣分数据上下文
        /// </summary>
        public DbSet<LostPoint> LostPoints { get; set; }

        /// <summary>
        /// 扣分项定义数据上下文
        /// </summary>
        public DbSet<LostPointDefine> LostPointDefines { get; set; }
    }
}
