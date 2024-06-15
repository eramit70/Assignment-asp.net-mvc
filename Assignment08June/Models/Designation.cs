using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Assignment08June.Models
{
    public class Designation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string DesignationName { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<DesignationGrade> DesignationGrades { get; set; }
    }
}