using System.ComponentModel.DataAnnotations;

namespace PReMaSys.Models
{
    public class PerformanceCriteria
    {
        [Key]
        public int PerformanceCriteriaId { get; set; }

        public string RewardsCriteria { get; set; }
    }

    public class PointsAllocation
    {
        [Key]
        public int PointsAllocationId { get; set; }
        public int PerformanceCriteriaId { get; set; }
        public string TimeLine { get; set; }
        public int PerformancePoints { get; set; }
        public int? CriteriaQuota { get; set; }

        public DateTime DateAded { get; set; }
        public DateTime? DateModified { get; set; }

    }
}
