using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Assignment08June.Models
{
    public class DesignationGrade
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Gradename { get; set; }

        [Required]
        public int DesignationIdref { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public virtual Designation Designation { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
    }
}