using System;
using System.Collections.Generic;
using EnrollmentSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Data;

public partial class EnrollmentSystemDbContext : DbContext
{
    public EnrollmentSystemDbContext()
    {
    }

    public EnrollmentSystemDbContext(DbContextOptions<EnrollmentSystemDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Announcement> Announcements { get; set; }

    public virtual DbSet<ApplicantAcademicRecord> ApplicantAcademicRecords { get; set; }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<ApplicationDocument> ApplicationDocuments { get; set; }

    public virtual DbSet<ApplicationParent> ApplicationParents { get; set; }

    public virtual DbSet<ApplicationSibling> ApplicationSiblings { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Curriculum> Curricula { get; set; }

    public virtual DbSet<CurriculumSubject> CurriculumSubjects { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<EducationalLevel> EducationalLevels { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<GradeLevel> GradeLevels { get; set; }

    public virtual DbSet<ProofOfPayment> ProofOfPayments { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<ScheduleDetail> ScheduleDetails { get; set; }

    public virtual DbSet<SchoolYear> SchoolYears { get; set; }

    public virtual DbSet<Section> Sections { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentCurriculum> StudentCurricula { get; set; }

    public virtual DbSet<StudentParent> StudentParents { get; set; }

    public virtual DbSet<StudentSchedule> StudentSchedules { get; set; }

    public virtual DbSet<StudentSibling> StudentSiblings { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=EnrollmentSystemDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admin__719FE48842FBC2CF");

            entity.HasOne(d => d.Employee).WithMany(p => p.Admins)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Admin_Employee");
        });

        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.HasKey(e => e.AnnouncementId).HasName("PK__Announce__9DE44574883A2F97");

            entity.Property(e => e.PostedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Section).WithMany(p => p.Announcements).HasConstraintName("FK_Announcement_Section");

            entity.HasOne(d => d.Subject).WithMany(p => p.Announcements).HasConstraintName("FK_Announcement_Subject");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Announcements).HasConstraintName("FK_Announcement_Teacher");
        });

        modelBuilder.Entity<ApplicantAcademicRecord>(entity =>
        {
            entity.HasKey(e => e.ApplicationAcademicRecordId).HasName("PK__Applican__0D6168D9AD127845");

            entity.HasOne(d => d.Application).WithMany(p => p.ApplicantAcademicRecords)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ApplicantAcademicRecord_Application");

            entity.HasOne(d => d.GradeLevel).WithMany(p => p.ApplicantAcademicRecords).HasConstraintName("FK_ApplicantAcademicRecord_GradeLevel");
        });

        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.ApplicationId).HasName("PK__Applicat__C93A4C99B8859CCF");

            entity.Property(e => e.ApplicationDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("Pending");

            entity.HasOne(d => d.GradeLevel).WithMany(p => p.Applications).HasConstraintName("FK_Application_GradeLevel");

            entity.HasOne(d => d.SchoolYear).WithMany(p => p.Applications).HasConstraintName("FK_Application_SchoolYear");
        });

        modelBuilder.Entity<ApplicationDocument>(entity =>
        {
            entity.HasKey(e => e.ApplicationDocumentId).HasName("PK__Applicat__9A4B1C6FCDEC2259");

            entity.HasOne(d => d.Application).WithMany(p => p.ApplicationDocuments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ApplicationDocument_Application");

            entity.HasOne(d => d.Document).WithMany(p => p.ApplicationDocuments).HasConstraintName("FK_ApplicationDocument_Documents");
        });

        modelBuilder.Entity<ApplicationParent>(entity =>
        {
            entity.HasKey(e => e.ApplicationParentId).HasName("PK__Applicat__86F89BADA0FA6C37");

            entity.HasOne(d => d.Application).WithMany(p => p.ApplicationParents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ApplicationParent_Application");
        });

        modelBuilder.Entity<ApplicationSibling>(entity =>
        {
            entity.HasKey(e => e.ApplicationSiblingId).HasName("PK__Applicat__E05E34FA893482CF");

            entity.HasOne(d => d.Application).WithMany(p => p.ApplicationSiblings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ApplicationSiblings_Application");
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetRo__3214EC07654F6608");
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetUs__3214EC07ACC26588");
        });

        modelBuilder.Entity<AspNetUserRole>(entity =>
        {
            entity.HasOne(d => d.Role).WithMany(p => p.AspNetUserRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AspNetUserRoles_Roles");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AspNetUserRoles_Users");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId).HasName("PK__AuditLog__EB5F6CBDE3C17AE7");

            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Student).WithMany(p => p.AuditLogs).HasConstraintName("FK_AuditLog_Student");
        });

        modelBuilder.Entity<Curriculum>(entity =>
        {
            entity.HasKey(e => e.CurriculumId).HasName("PK__Curricul__06C9FA1C4DA72D54");

            entity.HasOne(d => d.EducationalLevel).WithMany(p => p.Curricula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Curriculum_EducationalLevel");
        });

        modelBuilder.Entity<CurriculumSubject>(entity =>
        {
            entity.HasKey(e => e.CurriculumSubjectId).HasName("PK__Curricul__9D77DCA4AE8358E8");

            entity.HasOne(d => d.Curriculum).WithMany(p => p.CurriculumSubjects)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CurriculumSubject_Curriculum");

            entity.HasOne(d => d.GradeLevel).WithMany(p => p.CurriculumSubjects)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CurriculumSubject_GradeLevel");

            entity.HasOne(d => d.Subject).WithMany(p => p.CurriculumSubjects)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CurriculumSubject_Subject");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("PK__Document__1ABEEF0FFBB7FE1D");

            entity.HasOne(d => d.EducationalLevel).WithMany(p => p.Documents).HasConstraintName("FK_Documents_EducationalLevel");
        });

        modelBuilder.Entity<EducationalLevel>(entity =>
        {
            entity.HasKey(e => e.EducationalLevelId).HasName("PK__Educatio__CD37AB1086FFC626");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F11922A83BE");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK__Enrollme__7F68771B0B552529");

            entity.Property(e => e.Status).HasDefaultValue("Pending");

            entity.HasOne(d => d.Section).WithMany(p => p.Enrollments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrollment_Section");

            entity.HasOne(d => d.Student).WithMany(p => p.Enrollments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrollment_Student");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId).HasName("PK__Grades__54F87A57C7C6E113");

            entity.HasOne(d => d.GradeLevel).WithMany(p => p.Grades).HasConstraintName("FK_Grades_GradeLevel");

            entity.HasOne(d => d.Student).WithMany(p => p.Grades)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Grades_Student");

            entity.HasOne(d => d.Subject).WithMany(p => p.Grades)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Grades_Subject");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Grades).HasConstraintName("FK_Grades_Teacher");
        });

        modelBuilder.Entity<GradeLevel>(entity =>
        {
            entity.HasKey(e => e.GradeLevelId).HasName("PK__GradeLev__A200CF1389AA3F3C");

            entity.HasOne(d => d.EducationalLevel).WithMany(p => p.GradeLevels)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GradeLevel_EducationalLevel");
        });

        modelBuilder.Entity<ProofOfPayment>(entity =>
        {
            entity.HasKey(e => e.ProofOfPaymentId).HasName("PK__ProofOfP__17668496C119AB4B");

            entity.Property(e => e.Status).HasDefaultValue("Pending");

            entity.HasOne(d => d.Application).WithMany(p => p.ProofOfPayments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProofOfPayment_Application");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__Rooms__32863939FDF3A822");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Schedule__9C8A5B490FB198BD");

            entity.HasOne(d => d.Section).WithMany(p => p.Schedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_Section");

            entity.HasOne(d => d.Subject).WithMany(p => p.Schedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_Subject");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Schedules).HasConstraintName("FK_Schedule_Teacher");
        });

        modelBuilder.Entity<ScheduleDetail>(entity =>
        {
            entity.HasKey(e => e.ScheduleDetailId).HasName("PK__Schedule__921C9F153533AFE6");

            entity.HasOne(d => d.Room).WithMany(p => p.ScheduleDetails).HasConstraintName("FK_ScheduleDetails_Rooms");

            entity.HasOne(d => d.Schedule).WithMany(p => p.ScheduleDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ScheduleDetails_Schedule");
        });

        modelBuilder.Entity<SchoolYear>(entity =>
        {
            entity.HasKey(e => e.SchoolYearId).HasName("PK__SchoolYe__9BAABCE0AEE23269");
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasKey(e => e.SectionId).HasName("PK__Section__80EF0872BBFD259D");

            entity.HasOne(d => d.Curriculum).WithMany(p => p.Sections).HasConstraintName("FK_Section_Curriculum");

            entity.HasOne(d => d.GradeLevel).WithMany(p => p.Sections).HasConstraintName("FK_Section_GradeLevel");

            entity.HasOne(d => d.SchoolYear).WithMany(p => p.Sections).HasConstraintName("FK_Section_SchoolYear");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Student__32C52B99DA27C41A");

            entity.HasOne(d => d.Application).WithMany(p => p.Students).HasConstraintName("FK_Student_Application");
        });

        modelBuilder.Entity<StudentCurriculum>(entity =>
        {
            entity.HasKey(e => e.StudentCurriculumId).HasName("PK__StudentC__C68C7C8A53D2A6A0");

            entity.HasOne(d => d.Curriculum).WithMany(p => p.StudentCurricula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentCurriculum_Curriculum");

            entity.HasOne(d => d.SchoolYear).WithMany(p => p.StudentCurricula).HasConstraintName("FK_StudentCurriculum_SchoolYear");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentCurricula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentCurriculum_Student");
        });

        modelBuilder.Entity<StudentParent>(entity =>
        {
            entity.HasKey(e => e.StudentParentId).HasName("PK__StudentP__13C67664B5598779");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentParents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentParent_Student");
        });

        modelBuilder.Entity<StudentSchedule>(entity =>
        {
            entity.HasKey(e => e.StudentScheduleId).HasName("PK__StudentS__F54D364FDB4AE56E");

            entity.HasOne(d => d.Enrollment).WithMany(p => p.StudentSchedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentSchedule_Enrollment");

            entity.HasOne(d => d.Schedule).WithMany(p => p.StudentSchedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentSchedule_Schedule");
        });

        modelBuilder.Entity<StudentSibling>(entity =>
        {
            entity.HasKey(e => e.StudentSiblingId).HasName("PK__StudentS__B8F4615CF254714E");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentSiblings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentSiblings_Student");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__Subject__AC1BA3A8F970CADF");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("PK__Teacher__EDF259648129DDF2");

            entity.HasOne(d => d.Employee).WithMany(p => p.Teachers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Teacher_Employee");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
