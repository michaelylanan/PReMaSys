using PReMaSys.Data;

namespace PReMaSys.ViewModel
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsSelected { get; set; }
        public virtual ApplicationUser User { get; set; }

        
    }
}
