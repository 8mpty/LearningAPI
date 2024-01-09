using Microsoft.AspNetCore.Mvc;
using LearningAPI.Models;

namespace LearningAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly MyDbContext _context;

        public ScheduleController(MyDbContext context)
        {
            _context = context;
        }
        
        //private static readonly List<Schedule> list = new();

        [HttpGet]
        public IActionResult GetAll(string? search)
        {
            IQueryable<Schedule> result = _context.Schedules;
            if(search != null)
            {
                result = result.Where(x => x.Title.Contains(search) || x.Description.Contains(search));
            }
            var list = result.OrderByDescending(x => x.CreatedAt).ToList();
            return Ok(list);
        }

        [HttpPost]
        public IActionResult AddSchedule(Schedule schedule)
        {
            var now = DateTime.Now;
            var mySchedule = new Schedule()
            {
                Title = schedule.Title.Trim(),
                Description = schedule.Description.Trim(),
                CreatedAt = now,
                UpdatedAt = now,
            };

            _context.Schedules.Add(mySchedule);
            _context.SaveChanges();
            return Ok(schedule);
        }
    }
}
