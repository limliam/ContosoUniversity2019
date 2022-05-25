using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
        public DbSet<CourseAssignment> CourseAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Department>().Property(p => p.RowVersion).IsConcurrencyToken();
            modelBuilder.Entity<Instructor>().ToTable("Instructor");
            modelBuilder.Entity<OfficeAssignment>().ToTable("OfficeAssignment");
            modelBuilder.Entity<CourseAssignment>().ToTable("CourseAssignment");

            modelBuilder.Entity<CourseAssignment>()
                .HasKey(c => new { c.CourseID, c.InstructorID });
        }
    }
}

/*
The preceding code creates a DbSet property for each entity set. In EF terminology:
- An entity set typically corresponds to a database table.
- An entity corresponds to a row in the table.

The DbSet<Enrollment> and DbSet<Course> statements could be omitted and it would work the same. 
EF would include them implicitly because:
- The Student entity references the Enrollment entity.
- The Enrollment entity references the Course entity.

When the database is created, EF creates tables that have names the same as the DbSet property names. 
Property names for collections are typically plural. For example, Students rather than Student. 
Developers disagree about whether table names should be pluralized or not. 
For these tutorials, the default behavior is overridden by specifying singular table names in the DbContext. 
To do that, add the following highlighted code after the last DbSet property.
 */

/*
** Conventions **
The amount of code written in order for the EF to create a complete database is minimal because of the use of the conventions EF uses:

- The names of DbSet properties are used as table names. 
For entities not referenced by a DbSet property, entity class names are used as table names.
- Entity property names are used for column names.
- Entity properties that are named ID or classnameID are recognized as PK properties.
- A property is interpreted as a FK property if it's named <navigation property name><PK property name>. 
For example, StudentID for the Student navigation property since the Student entity's PK is ID. 
FK properties can also be named <primary key property name>. 
For example, EnrollmentID since the Enrollment entity's PK is EnrollmentID.

Conventional behavior can be overridden. 
For example, table names can be explicitly specified, as shown earlier in this tutorial. 
Column names and any property can be set as a PK or FK.
 */ 