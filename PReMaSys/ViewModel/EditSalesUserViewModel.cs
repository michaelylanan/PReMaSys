using System.ComponentModel.DataAnnotations;

namespace PReMaSys.ViewModel
{
    public class EditSalesUserViewModel
    {
        public EditSalesUserViewModel()
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
    }
}
