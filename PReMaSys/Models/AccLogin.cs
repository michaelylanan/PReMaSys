using System.ComponentModel.DataAnnotations;

namespace PReMaSys.Models
{
    public class AccLogin
    {
        [Key]
        public int accID { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Admin Email")]
        public string? AdminEmail { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Password")]
        public string? password { get; set; }
    }
}
