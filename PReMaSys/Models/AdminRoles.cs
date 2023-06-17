using System.ComponentModel.DataAnnotations;
using PReMaSys.Data;

namespace PReMaSys.Models
{
    public class AdminRoles
    {
        [Key]
        public int AdminRoleId { get; set; }

        //Foreign Key of Id in AspNetUsers
        public virtual ApplicationUser ApplicationUser { get; set; }

        public Role Role { get; set; }

        [Display(Name = "Picture")]
        public string AdminPicture { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Admin Employee No.")]
        public string AdminEmployeeNo { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Admin First Name")]
        public string AdminFirstname { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Admin Last Name")]
        public string AdminLastname { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Admin Email")]
        public string AdminEmail { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Contact No.")]
        [StringLength(11, ErrorMessage = "Phone number must be 11 Digits!")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[.]?([0-9]{4})[. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid. Must be 11 digits and numeric")]
        public string ContactNo { get; set; }

        [Required(ErrorMessage = "Required")]
        public string Password { get; set; }

        [Display(Name = "Date Added")]
        public DateTime DateAdded { get; set; }

        [Display(Name = "Date Modified")]
        public DateTime? DateModified { get; set; }


    }
    public enum Role
    {
        Admin = 1,
        Support = 2,
    }
}
