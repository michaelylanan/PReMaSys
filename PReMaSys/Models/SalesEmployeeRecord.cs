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
    public class SalesEmployeeRecord
    {
        [Key]
        public int SEmployeeRecordsID { get; set; }
        //Foreign Key of Id in AspNetUsers
        public virtual ApplicationUser? ApplicationUser { get; set; }

        [Required(ErrorMessage ="Required")]
        [Display(Name = "Employee No.")]
        public string EmployeeNo { get; set; }

        [Display(Name = "Image")]
        public string EmployeePicture { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Employee Username")]
        public string EmployeeUsername { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Employee First Name")]
        public string EmployeeFirstname { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Employee Last Name")]
        public string EmployeeLastname { get; set; }

        [Required(ErrorMessage = "Required")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Employee Address")]
        public string EmployeeAddress { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Employee Birthdate")]
        public string EmployeeBirthdate { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Employee Email")]
        public string EmployeeEmail { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Contact No.")]
        [StringLength(11, ErrorMessage = "Phone number must be 11 Digits!")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[.]?([0-9]{4})[. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid. Must be 11 digits and numeric")]
        public string ContactNo { get; set; }

        [Required(ErrorMessage = "Required")]
        public string Password { get; set; }


        [Range(0, 10000000000, ErrorMessage = "Must be a valid number!")]
        [Display(Name = "Employee Points")]
        public string? EmployeePoints { get; set; }

        [Display(Name ="Date Added")]
        public DateTime DateAdded { get; set; }

        [Display(Name ="Date Modified")]
        public DateTime? DateModified { get; set; }
    }

    public enum Gender
    {
        Male =1,
        Female=2,
    }

}
