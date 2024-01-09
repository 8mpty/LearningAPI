using System.ComponentModel.DataAnnotations;

namespace LearningAPI.Models
{
    public class UpdateScheduleRequest
    {
        [MinLength(6), MaxLength(100)]
        public string? Title { get; set; }
        [MinLength(6), MaxLength(100)]
        public string? Description { get; set; }
        [MaxLength(20)]
        public string? ImageFile { get; set; }
    }
}
