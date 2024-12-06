using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcq.Models
{
    public class McqsDbContext : DbContext
    {
        public McqsDbContext(DbContextOptions<McqsDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<ExamAttempt> ExamAttempts { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }
        // Add other DbSets here
        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("USER");

            // Chuyển đổi GUID thành Binary(16)
            entity.Property(e => e.Iduser)
                .HasColumnType("binary(16)")
                .HasConversion(
                    v => v.ToByteArray(),     // GUID -> binary(16)
                    v => new Guid(v));        // binary(16) -> GUID
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Teacher>(entity =>
        {

            entity.HasOne(t => t.User)
            .WithOne() 
            .HasForeignKey<Teacher>(t => t.Idteacher)
            .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.Idteacher)
                .HasColumnType("binary(16)")
                .HasConversion(
                    v => v.ToByteArray(),
                    v => new Guid(v));
            
            
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.Property(e => e.Idstudent)
                .HasColumnType("binary(16)")
                .HasConversion(
                    v => v.ToByteArray(),
                    v => new Guid(v));
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.Property(e => e.Idstudent)
                .HasColumnType("binary(16)")
                .HasConversion(
                    v => v.ToByteArray(),     // GUID -> binary(16)
                    v => new Guid(v));        // binary(16) -> GUID
        });

        // Cấu hình bảng TOPIC
        modelBuilder.Entity<Topic>(entity =>
        {
            entity.Property(e => e.Idtopic)
                .HasColumnType("binary(16)")
                .HasConversion(
                    v => v.ToByteArray(),     // GUID -> binary(16)
                    v => new Guid(v));        // binary(16) -> GUID

            entity.Property(e => e.Idproperty)
                .HasColumnType("binary(16)")
                .HasConversion(
                    v => v.ToByteArray(),     // GUID -> binary(16)
                    v => new Guid(v));        // binary(16) -> GUID

            entity.Property(e => e.Idteacher)
                .HasColumnType("binary(16)")
                .HasConversion(
                    v => v.ToByteArray(),     // GUID -> binary(16)
                    v => new Guid(v));        // binary(16) -> GUID

            entity.HasOne(t => t.Property)
                .WithMany()
                .HasForeignKey(t => t.Idproperty)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Teacher)
                .WithMany()
                .HasForeignKey(t => t.Idteacher)
                .OnDelete(DeleteBehavior.SetNull);
        });



    // Cấu hình bảng EXAM
        modelBuilder.Entity<Exam>(entity =>
        {
            entity.Property(e => e.Idexam)
                .HasColumnType("binary(16)")
                .HasConversion(
                    v => v.ToByteArray(),     // GUID -> binary(16)
                    v => new Guid(v));        // binary(16) -> GUID

            entity.Property(e => e.Idteacher)
                .HasColumnType("binary(16)")
                .HasConversion(
                    v => v.ToByteArray(),     // GUID -> binary(16)
                    v => new Guid(v));        // binary(16) -> GUID

            entity.Property(e => e.Idtopic)
                .HasColumnType("binary(16)")
                .HasConversion(
                    v => v.ToByteArray(),     // GUID -> binary(16)
                    v => new Guid(v));        // binary(16) -> GUID

            entity.HasOne(e => e.Teacher)
                .WithMany()
                .HasForeignKey(e => e.Idteacher)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Topic)
                .WithMany()
                .HasForeignKey(e => e.Idtopic)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Cấu hình bảng QUESTION
        modelBuilder.Entity<Question>(entity =>
        {
            entity.Property(e => e.Idquestion)
                .HasColumnType("binary(16)")
                .HasConversion(
                    v => v.ToByteArray(),     // GUID -> binary(16)
                    v => new Guid(v));        // binary(16) -> GUID

            entity.Property(e => e.Idexam)
                .HasColumnType("binary(16)")
                .HasConversion(
                    v => v.ToByteArray(),     // GUID -> binary(16)
                    v => new Guid(v));        // binary(16) -> GUID

            entity.HasOne(q => q.Exam)
                .WithMany(e => e.Questions)
                .HasForeignKey(q => q.Idexam)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Cấu hình bảng QUESTIONOPTION
        modelBuilder.Entity<QuestionOption>()
            .HasKey(qo => new { qo.Idquestion, qo.option_content });

        modelBuilder.Entity<QuestionOption>()
            .HasOne(qo => qo.Question)
            .WithMany(q => q.QuestionOptions)
            .HasForeignKey(qo => qo.Idquestion)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ExamAttempt>()
            .HasKey(e => e.Idattempt);

        modelBuilder.Entity<ExamAttempt>()
            .HasOne(e => e.Student)
            .WithMany()
            .HasForeignKey(e => e.Idstudent)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ExamAttempt>()
            .HasOne(e => e.Exam)
            .WithMany()
            .HasForeignKey(e => e.Idexam)
            .OnDelete(DeleteBehavior.Cascade);
    
        modelBuilder.Entity<StudentAnswer>()
            .HasKey(sa => new { sa.Idattempt, sa.Idquestion, sa.SelectedAns });

        modelBuilder.Entity<StudentAnswer>()
            .HasOne(sa => sa.ExamAttempt)
            .WithMany(ea => ea.StudentAnswers)
            .HasForeignKey(sa => sa.Idattempt)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StudentAnswer>()
            .HasOne(sa => sa.Question)
            .WithMany(q => q.StudentAnswers)
            .HasForeignKey(sa => sa.Idquestion)
            .OnDelete(DeleteBehavior.Cascade);
    


    
    }

}

    [Table("USER")]
    public class User
    {
        [Key]
        [Column("iduser")]
        public Guid Iduser { get; set; } // Tự động sinh UUID

        [Required(ErrorMessage = "First name is required")]
        [Column("first_name")]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [Column("last_name")]
        [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Column("email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [Column("password")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [Column("phonenumber")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [Column("date_of_birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Column("create_account_date")]
        public DateTime CreateAccountDate { get; set; } // Tự động sinh

        [Column("update_info_date")]
        public DateTime? UpdateInfoDate { get; set; }

        [Required(ErrorMessage = "Role ID is required")]
        [Column("idrole")]
        public int Idrole { get; set; }
    }



    public class Role
    {
        [Key]
        public int Idrole { get; set; }
        public string RoleName { get; set; }
    }
    [Table("STUDENT")]
    public class Student
    {
        [Key]
        [Column("idstudent")]
        public Guid Idstudent { get; set; } // Sửa thành Guid

        [Column("grade")]
        public int? Grade { get; set; }

        [Column("school_name")]
        public string? SchoolName { get; set; }
    }

    [Table("TEACHER")]
    public class Teacher
    {
        [Key]
        [Column("idteacher")]
        public Guid Idteacher { get; set; } // Sửa thành Guid

        [Column("specialization")]
        public string? Specialization { get; set; }

        [Column("qualification")]
        public string? Qualification { get; set; }

        [Column("department")]
        public string? Department { get; set; }

        public User User { get; set; }

    }

    [Table("role_permission")]
    public class RolePermission
    {
        [Key]
        [Column("idrole")]
        public int Idrole { get; set; }

        [Column("idresource")]
        public int Idresource { get; set; }

        [Column("idpermission")]
        public int PermissionLevel { get; set; }
    }

    [Table("resource")]
    public class Resource
    {
        [Key]
        [Column("idresource")]
        public int Idresource { get; set; }

        [Column("name")]
        public string ResourceName { get; set; }
    }

    [Table("EXAM")]
    public class Exam
    {
        [Key]
        public Guid Idexam { get; set; }

        [Required]
        [MaxLength(225)]
        public string Name { get; set; }

        [Required]
        public int Duration { get; set; }

        [Required]
        public int number_of_question { get; set; }

        public Guid Idteacher { get; set; }
        public Teacher Teacher { get; set; }

        [Required]
        public Guid Idtopic { get; set; }
        public Topic Topic { get; set; }

        public DateTime created_time { get; set; }

        public ICollection<Question> Questions { get; set; }
    }

    [Table("QUESTION")]
    public class Question
    {
        [Key]
        public Guid Idquestion { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        [MaxLength(30)]
        public string difficulty_level { get; set; }

        [Required]
        public Guid Idexam { get; set; }
        public Exam Exam { get; set; }

        public ICollection<QuestionOption> QuestionOptions { get; set; }
        public virtual ICollection<StudentAnswer> StudentAnswers { get; set; }

    }

    [Table("QUESTION_OPTION")]
    public class QuestionOption
    {
        [Required]
        public Guid Idquestion { get; set; }
        public Question Question { get; set; }

        [Required]
        [MaxLength(225)]
        public string option_content { get; set; }

        [Required]
        public bool IsCorrect { get; set; }

        public string Explanation { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string UniqueKey => $"{Idquestion}-{option_content}";
    }

    [Table("TOPIC")]
    public class Topic
    {
        [Key]
        public Guid Idtopic { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public Guid Idproperty { get; set; }

        public Guid Idteacher { get; set; }
        public DateTime created_date { get; set; }
        
        public Property Property { get; set; }
        public Teacher Teacher { get; set; }
    }

    [Table("PROPERTY")]
    public class Property
    {
        [Key]
        public Guid Idproperty { get; set; }

        [Required]
        public int Grade { get; set; }

        [Required]
        [MaxLength(100)]
        public string Subject { get; set; }
    }

    [Table("EXAM_ATTEMPT")]
    public class ExamAttempt
    {
        [Key]
        [Column("idattempt", TypeName = "BINARY(16)")]
        public Guid Idattempt { get; set; }

        [Column("start_time")]
        public DateTime StartTime { get; set; }

        [Column("completion_time")]
        public DateTime CompletionTime { get; set; }

        [Column("exam_score", TypeName = "decimal(10,0)")]
        public decimal ExamScore { get; set; }

        [Column("nums_correct_ans")]
        public int NumsCorrectAns { get; set; }

        [Column("idstudent", TypeName = "BINARY(16)")]
        public Guid Idstudent { get; set; }

        [Column("idexam", TypeName = "BINARY(16)")]
        public Guid Idexam { get; set; }

        [ForeignKey("Idstudent")]
        public virtual Student Student { get; set; }

        [ForeignKey("Idexam")]
        public virtual Exam Exam { get; set; }

        public virtual ICollection<StudentAnswer> StudentAnswers { get; set; }
    }

    [Table("STUDENT_ANSWER")]
    public class StudentAnswer
    {
        [Key]
        [Column("idattempt", TypeName = "BINARY(16)")]
        public Guid Idattempt { get; set; }

        [Key]
        [Column("idquestion", TypeName = "BINARY(16)")]
        public Guid Idquestion { get; set; }

        [Key]
        [Column("selected_ans", TypeName = "VARCHAR(255)")]
        [MaxLength(255)]
        public string SelectedAns { get; set; }

        [Column("iscorrect")]
        public bool IsCorrect { get; set; }

        [ForeignKey("Idattempt")]
        public virtual ExamAttempt ExamAttempt { get; set; }

        [ForeignKey("Idquestion")]
        public virtual Question Question { get; set; }
    }

}
