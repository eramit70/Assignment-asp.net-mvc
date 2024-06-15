using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Assignment08June.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Firstname { get; set; }



        public string Lastname { get; set; } = "";

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string EmailAddress { get; set; }


        public string Phonenumber { get; set; } = "";

       
        public int DesignationIdRef { get; set; }

        
        public int GradeIdref { get; set; }

        public virtual Designation Designation { get; set; }
        public virtual DesignationGrade DesignationGrade { get; set; }
    }
}