using PReMaSys.Data;
using System.ComponentModel.DataAnnotations;

namespace PReMaSys.Models
{
    public class Rewards
    {
        [Key]
        public int RewardsInformationId { get; set; }
        //Foreign Key of Id in AspNetUsers
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "Image")]
        public string Picture { get; set; }

        [Required(ErrorMessage = "Required.")]
        [Display(Name = "Reward Name")]
        public string RewardName { get; set; }

        [Required(ErrorMessage = "Required*")]
        public Category Category { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Range(0.00, 100000.00, ErrorMessage = "Invalid Cost Range.")]
        [Display(Name = "Reward Cost")]
        public decimal RewardCost { get; set; }

        [Range(0.00, 100000.00, ErrorMessage = "Invalid Point Cost Range.")]
        [Display(Name = "Point Cost")]
        public decimal PointsCost { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Invalid Quantity")]
        public int Quantity { get; set; }

        public Status? Status { get; set; }

        [Display(Name = "Date Added")]
        public DateTime DateAdded { get; set; }

        [Display(Name = "Date Modified")]
        public DateTime? DateModified { get; set; }

        public string? SupportId { get; set; }

        /* [Required(ErrorMessage ="Required.")]
         public virtual Category Category { get; set; }*/

    }

    public enum Category
    {
        Food = 1,
        Travel = 2,
        Discounts = 3,
        Others = 4,
    }

    public enum Status
    {
        OnReview = 1,
        Approved = 2,
        Reject = 3,
    }

    public class AddToCart
    {
        [Key]
        public int CartId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public Rewards Reward { get; set; }

        [Display(Name = "Reward Image")]
        public string RewardImage { get; set; }

        [Display(Name = "Reward Name")]
        public string RewardName { get; set; }
        public Category Category { get; set; }

        [Display(Name = "Reward Description")]
        public string RewardDescription { get; set; }

        [Display(Name = "Reward Price")]
        public decimal RewardPrice { get; set; }

        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid Quantity.")]
        public int Quantity { get; set; }
    }

}
