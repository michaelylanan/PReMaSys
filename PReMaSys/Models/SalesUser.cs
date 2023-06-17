using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PReMaSys.Models
{
    public class SalesUser
    {
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Employee No.")]
        public string EmployeeNo { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Employee First Name")]
        public string EmployeeFirstname { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Employee Last Name")]
        public string EmployeeLastname { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Employee Address")]
        public string EmployeeAddress { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Employee Birthdate")]
        public string EmployeeBirthdate { get; set; }

        //Login Credentials


        [Required]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required]
        [Display(Name = "Password")]

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }
}
