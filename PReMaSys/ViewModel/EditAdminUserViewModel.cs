using PReMaSys.Models;
using System.ComponentModel.DataAnnotations;

namespace PReMaSys.ViewModel
{
    public class EditAdminUserViewModel
    {
        public EditAdminUserViewModel()
        {
            Roles = new List<string>();
        }

        public string Id { get; set; }

        

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /*public List<string> Claims { get; set; }*/
        public IList<string> Roles { get; set; }


        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string? Password { get; set; }

        
    }
}
