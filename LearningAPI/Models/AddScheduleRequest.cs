using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningAPI.Models
{
    public class AddScheduleRequest
    {
        [Required, MinLength(6), MaxLength(100)]
        public string Title { get; set; } = string.Empty;
        [Required, MinLength(6), MaxLength(100)]
        public string Description { get; set; } = string.Empty;

        [Required, Column(TypeName = "datetime")]
        public DateTime SelectedDate { get; set; }

        [Required, Column(TypeName = "datetime")]
        public DateTime SelectedTime { get; set; }
        [MaxLength(20)]
        public string? ImageFile { get; set; }
    }
}
