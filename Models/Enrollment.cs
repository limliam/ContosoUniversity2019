using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public enum Grade
    {
        A, B, C, D, F
    }

    public class Enrollment
    {
        public int EnrollmentID { get; set; }
        public int CourseID { get; set; }
        public int StudentID { get; set; }
        [DisplayFormat(NullDisplayText = "No grade")]
        public Grade? Grade { get; set; }

        public Course Course { get; set; }
        public Student Student { get; set; }
    }
}

/*
 The EnrollmentID property is the PK. This entity uses the classnameID pattern instead of ID by itself. 
The Student entity used the ID pattern. Some developers prefer to use one pattern throughout the data model. 
In this tutorial, the variation illustrates that either pattern can be used. 
A later tutorial shows how using ID without classname makes it easier to implement inheritance in the data model.

The Grade property is an enum. The ? after the Grade type declaration indicates that the Grade property is nullable. 
A grade that's null is different from a zero grade. null means a grade isn't known or hasn't been assigned yet.

The StudentID property is a foreign key (FK), and the corresponding navigation property is Student. 
An Enrollment entity is associated with one Student entity, so the property can only hold a single Student entity. 
This differs from the Student.Enrollments navigation property, which can hold multiple Enrollment entities.

The CourseID property is a FK, and the corresponding navigation property is Course. 
An Enrollment entity is associated with one Course entity.

Entity Framework interprets a property as a FK property if it's named <navigation property name><primary key property name>. 
For example, StudentID for the Student navigation property since the Student entity's PK is ID. 
FK properties can also be named <primary key property name>. For example, CourseID because the Course entity's PK is CourseID.
 */ 