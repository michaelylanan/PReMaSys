using System.ComponentModel.DataAnnotations;

namespace PReMaSys.Models
{
    public class AccLoginSE
    {
        [Key]
        public int accID { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Employee Email")]
        public string? EmployeeEmail { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Password")]
        public string? Password { get; set; }
    }
}
