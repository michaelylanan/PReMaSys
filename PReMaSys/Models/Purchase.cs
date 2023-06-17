
using PReMaSys.Data;
using System.ComponentModel.DataAnnotations;

namespace PReMaSys.Models
{
    public class Purchase
    {

        [Key]
        public int PurchaseId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
        public virtual AddToCart? AddToCart { get; set; }

        [Display(Name = "Employee Name")]
        public string? EmployeeName { get; set; }

        [Display(Name = "Reward Image")]
        public string? RewardImage { get; set; }

        [Display(Name = "Reward Name")]
        public string? RewardName { get; set; }

        [Display(Name = "Reward Price")]
        public decimal? RewardPrice { get; set; }

        [Display(Name = "Total Payment")]
        public decimal? TotalPayment { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateModified { get; set; }
        public Stat? Stat { get; set; }

    }
    public enum Stat
    {
        Processing = 1,
        ClaimRewardNow = 2,
        Unclaimed = 3,
        Claimed = 4,
    }

}
