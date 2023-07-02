using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using PReMaSys.Data;

namespace PReMaSys.Models
{
    public class SERecord
    {
        [Key]
        public int SEmployeeRecordsID { get; set; }

        public ApplicationUser?  SERId { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Employee No.")]
        public string? EmployeeNo { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Employee First Name")]
        public string? EmployeeFirstname { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Employee Last Name")]
        public string? EmployeeLastname { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Employee Address")]
        public string? EmployeeAddress { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Employee Birthdate")]
        public string? EmployeeBirthdate { get; set; }


        [Range(0, 10000000000, ErrorMessage = "Must be a valid number!")]
        [Display(Name = "Employee Points")]
        public int? EmployeePoints { get; set; }

        [Display(Name = "Date Added")]
        public DateTime DateAdded { get; set; }

        [Display(Name = "Date Modified")]
        public DateTime? DateModified { get; set; }

        public DateTime? IsArchived { get; set; }
    }
}

