using System.ComponentModel.DataAnnotations;

namespace PReMaSys.Models
{
    public class DomainAccount
    {
        [Key]
        public int AdminInfoId { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        //[Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Company Logo")]
        public string CompanyLogo { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Company Address")]
        public string CompanyAddress { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Business Size")]
        public BusinessSize BusinessSize { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Nature of Business")]
        public string NatureOfBusiness { get; set; }


        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Company Birthday")]
        public string CompanyBday { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Domain Admin Picture")]
        public string AdminPicture { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Domain Admin Last Name")]
        public string AdminLastname { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Domain Admin First Name")]
        public string AdminFirstname { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Contact No.")]
        public string ContactNo { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Domain Admin Email")]
        public string AdminEmail { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Display(Name = "Date Added")]
        public DateTime DateAdded { get; set; }

        [Display(Name = "Date Modified")]
        public DateTime? DateModified { get; set; }
    }

    public enum BusinessSize
    {
        Micro = 1,
        Small = 2,
        Medium = 3,
        Large = 4,
    }
}

