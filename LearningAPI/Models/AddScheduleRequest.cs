using System.ComponentModel.DataAnnotations;
namespace LearningAPI.Models
{
    public class AddScheduleRequest
    {
        [Required, MinLength(6), MaxLength(100)]
        public string Title { get; set; } = string.Empty;
        [Required, MinLength(6), MaxLength(100)]
        public string Description { get; set; } = string.Empty;
        [MaxLength(20)]
        public string? ImageFile { get; set; }
    }
}
