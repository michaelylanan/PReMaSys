using System.ComponentModel.DataAnnotations;

namespace PReMaSys.Models
{
    public class PointsTracker
    {
        [Key]
        public int PointsTrackerId { get; set; }
        public int PerformanceCriteriaId { get; set; }
        public string SalesPerson { get; set; }
        public string TimeLine { get; set; }
        public int PerformancePoints { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime? DateModified { get; set; }

    }
}
