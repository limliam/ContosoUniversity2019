using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Student
    {
        public int ID { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [StringLength(50)]
        [Column("FirstName")]
        [Display(Name = "First Name")]
        public string FirstMidName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Enrollment Date")]
        public DateTime EnrollmentDate { get; set; }
        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstMidName;
            }
        }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}

/*
The ID property is the primary key (PK) column of the database table that corresponds to this class. 
By default, EF interprets a property that's named ID or classnameID as the primary key. 
For example, the PK could be named StudentID rather than ID.

The Enrollments property is a navigation property. Navigation properties hold other entities that are related to this entity. 
The Enrollments property of a Student entity:

Contains all of the Enrollment entities that are related to that Student entity.
If a specific Student row in the database has two related Enrollment rows:
That Student entity's Enrollments navigation property contains those two Enrollment entities.
Enrollment rows contain a student's PK value in the StudentID foreign key (FK) column.

If a navigation property can hold multiple entities:

The type must be a list, such as ICollection<T>, List<T>, or HashSet<T>.
Entities can be added, deleted, and updated.
Many-to-many and one-to-many navigation relationships can contain multiple entities. 
When ICollection<T> is used, EF creates a HashSet<T> collection by default.
*/
