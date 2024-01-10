using System.ComponentModel.DataAnnotations.Schema;

namespace LearningAPI.Models
{
    public class ScheduleDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime SelectedDate { get; set; }
        public DateTime SelectedTime { get; set; }
        public string? ImageFile { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UserId { get; set; }
        public UserBasicDTO? User { get; set; }
    }
}
