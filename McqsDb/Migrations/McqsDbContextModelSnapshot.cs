﻿// <auto-generated />
using System;
using Mcq.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace McqsDb.Migrations
{
    [DbContext(typeof(McqsDbContext))]
    partial class McqsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Mcq.Models.Role", b =>
                {
                    b.Property<int>("Idrole")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("idrole");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Idrole"));

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("role_name");

                    b.HasKey("Idrole");

                    b.ToTable("ROLE", (string)null);
                });

            modelBuilder.Entity("Mcq.Models.Student", b =>
                {
                    b.Property<byte[]>("Idstudent")
                        .HasColumnType("BINARY(16)")
                        .HasColumnName("idstudent");

                    b.Property<int?>("Grade")
                        .HasColumnType("int")
                        .HasColumnName("grade");

                    b.Property<string>("SchoolName")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("school_name");

                    b.HasKey("Idstudent");

                    b.ToTable("STUDENT", (string)null);
                });

            modelBuilder.Entity("Mcq.Models.Teacher", b =>
                {
                    b.Property<byte[]>("Idteacher")
                        .HasColumnType("BINARY(16)")
                        .HasColumnName("idteacher");

                    b.Property<string>("Department")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("department");

                    b.Property<string>("Qualification")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("qualification");

                    b.Property<string>("Specialization")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("specialization");

                    b.HasKey("Idteacher");

                    b.ToTable("TEACHER", (string)null);
                });

            modelBuilder.Entity("Mcq.Models.User", b =>
                {
                    b.Property<byte[]>("Iduser")
                        .HasColumnType("BINARY(16)")
                        .HasColumnName("iduser");

                    b.Property<DateTime>("CreateAccountDate")
                        .HasColumnType("DATETIME")
                        .HasColumnName("create_account_date");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("DATETIME")
                        .HasColumnName("date_of_birth");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("email");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("first_name");

                    b.Property<int>("Idrole")
                        .HasColumnType("int")
                        .HasColumnName("idrole");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("last_name");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("password");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(15)
                        .HasColumnType("varchar(15)")
                        .HasColumnName("phonenumber");

                    b.Property<DateTime?>("UpdateInfoDate")
                        .HasColumnType("DATETIME")
                        .HasColumnName("update_info_date");

                    b.HasKey("Iduser");

                    b.ToTable("USER", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}