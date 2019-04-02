﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RedisStudy.Data;

namespace RedisStudy.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190322050734_add_examorder1")]
    partial class add_examorder1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<string>", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(127);

                    b.Property<string>("ConcurrencyStamp");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Name");

                    b.Property<string>("NormalizedName");

                    b.HasKey("Id");

                    b.ToTable("IdentityRole<string>");

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityRole<string>");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId");

                    b.HasKey("Id");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser<string>", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(127);

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Email");

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail");

                    b.Property<string>("NormalizedUserName");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("IdentityUser<string>");

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityUser<string>");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(127);

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(127);

                    b.HasKey("UserId");

                    b.ToTable("UserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasMaxLength(127);

                    b.Property<string>("RoleId")
                        .HasMaxLength(127);

                    b.HasKey("UserId", "RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(127);

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(127);

                    b.Property<string>("Name")
                        .HasMaxLength(127);

                    b.Property<string>("Value");

                    b.HasKey("UserId");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("RedisStudy.Data.Entities.AuthType", b =>
                {
                    b.Property<int>("AuthTypeID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AuthTypeName")
                        .HasMaxLength(200);

                    b.Property<bool>("InUse");

                    b.HasKey("AuthTypeID");

                    b.ToTable("DE_AuthType");
                });

            modelBuilder.Entity("RedisStudy.Data.Entities.Exam", b =>
                {
                    b.Property<long>("StudentID");

                    b.Property<int>("Order");

                    b.Property<DateTime?>("ExamBegin");

                    b.Property<DateTime?>("ExamEnd");

                    b.HasKey("StudentID", "Order");

                    b.ToTable("DE_Exam");
                });

            modelBuilder.Entity("RedisStudy.Data.Entities.ExamCar", b =>
                {
                    b.Property<int>("ExamCarID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CarNumber")
                        .HasMaxLength(20);

                    b.Property<string>("CarPlat")
                        .HasMaxLength(20);

                    b.Property<string>("CarStyle")
                        .HasMaxLength(20);

                    b.Property<string>("SubStyle")
                        .HasMaxLength(20);

                    b.HasKey("ExamCarID");

                    b.ToTable("DE_ExamCar");
                });

            modelBuilder.Entity("RedisStudy.Data.Entities.ExamOrder", b =>
                {
                    b.Property<int>("ExamOrderID")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ExamProjectID");

                    b.Property<int>("Order");

                    b.HasKey("ExamOrderID");

                    b.HasIndex("ExamProjectID");

                    b.ToTable("DE_ExamOrder");
                });

            modelBuilder.Entity("RedisStudy.Data.Entities.ExamProject", b =>
                {
                    b.Property<long>("ExamProjectID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ExamProjectCode")
                        .HasMaxLength(100);

                    b.Property<string>("ExamProjectName")
                        .HasMaxLength(100);

                    b.HasKey("ExamProjectID");

                    b.ToTable("DE_ExamProject");
                });

            modelBuilder.Entity("RedisStudy.Data.Entities.LostPoint", b =>
                {
                    b.Property<long>("StudentID");

                    b.Property<int>("Order");

                    b.Property<long>("LostPointDefineID");

                    b.HasKey("StudentID", "Order", "LostPointDefineID");

                    b.HasIndex("LostPointDefineID");

                    b.ToTable("DE_LostPoint");
                });

            modelBuilder.Entity("RedisStudy.Data.Entities.LostPointDefine", b =>
                {
                    b.Property<long>("LostPointDefineID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("LostPoint");

                    b.Property<string>("LostPointCode")
                        .HasMaxLength(100);

                    b.Property<string>("Reason")
                        .HasMaxLength(100);

                    b.HasKey("LostPointDefineID");

                    b.ToTable("DE_LostPointDefine");
                });

            modelBuilder.Entity("RedisStudy.Data.Entities.ProjectLostPoint", b =>
                {
                    b.Property<long>("StudentID");

                    b.Property<int>("Order");

                    b.Property<long>("ExamProjectID");

                    b.Property<long>("LostPointDefineID");

                    b.HasKey("StudentID", "Order", "ExamProjectID", "LostPointDefineID");

                    b.HasIndex("LostPointDefineID");

                    b.ToTable("DE_ProjectLostPoint");
                });

            modelBuilder.Entity("RedisStudy.Data.Entities.Student", b =>
                {
                    b.Property<long>("StudentID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AuthCode")
                        .HasMaxLength(100);

                    b.Property<int>("AuthTypeID");

                    b.Property<string>("CarStyle");

                    b.Property<int>("ExamCarID");

                    b.Property<bool?>("Passed");

                    b.Property<int?>("Score");

                    b.Property<string>("StudentName")
                        .HasMaxLength(40);

                    b.Property<string>("SubCarStyle");

                    b.HasKey("StudentID");

                    b.HasIndex("AuthTypeID");

                    b.HasIndex("ExamCarID");

                    b.ToTable("DE_Stuendt");
                });

            modelBuilder.Entity("RedisStudy.Data.Entities.StudentExamProject", b =>
                {
                    b.Property<long>("StudentID");

                    b.Property<int>("Order");

                    b.Property<long>("ExamProjectID");

                    b.Property<DateTime?>("ExamBegin");

                    b.Property<DateTime?>("ExamEnd");

                    b.HasKey("StudentID", "Order", "ExamProjectID");

                    b.HasIndex("ExamProjectID");

                    b.ToTable("DE_StudentExamProject");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityRole<string>");

                    b.HasDiscriminator().HasValue("IdentityRole");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityUser<string>");

                    b.HasDiscriminator().HasValue("IdentityUser");
                });

            modelBuilder.Entity("RedisStudy.Data.Entities.Exam", b =>
                {
                    b.HasOne("RedisStudy.Data.Entities.Student", "Belong")
                        .WithMany("Exams")
                        .HasForeignKey("StudentID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RedisStudy.Data.Entities.ExamOrder", b =>
                {
                    b.HasOne("RedisStudy.Data.Entities.ExamProject", "Project")
                        .WithMany()
                        .HasForeignKey("ExamProjectID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RedisStudy.Data.Entities.LostPoint", b =>
                {
                    b.HasOne("RedisStudy.Data.Entities.LostPointDefine", "Define")
                        .WithMany()
                        .HasForeignKey("LostPointDefineID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RedisStudy.Data.Entities.Exam", "BelongExam")
                        .WithMany("LostPoints")
                        .HasForeignKey("StudentID", "Order")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RedisStudy.Data.Entities.ProjectLostPoint", b =>
                {
                    b.HasOne("RedisStudy.Data.Entities.LostPointDefine", "Define")
                        .WithMany()
                        .HasForeignKey("LostPointDefineID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RedisStudy.Data.Entities.StudentExamProject", "Project")
                        .WithMany("LostPoints")
                        .HasForeignKey("StudentID", "Order", "ExamProjectID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RedisStudy.Data.Entities.Student", b =>
                {
                    b.HasOne("RedisStudy.Data.Entities.AuthType", "StudentAuthType")
                        .WithMany()
                        .HasForeignKey("AuthTypeID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RedisStudy.Data.Entities.ExamCar", "Car")
                        .WithMany()
                        .HasForeignKey("ExamCarID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RedisStudy.Data.Entities.StudentExamProject", b =>
                {
                    b.HasOne("RedisStudy.Data.Entities.ExamProject", "CurrentProject")
                        .WithMany()
                        .HasForeignKey("ExamProjectID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RedisStudy.Data.Entities.Exam", "BelongExam")
                        .WithMany("ExamProjects")
                        .HasForeignKey("StudentID", "Order")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
