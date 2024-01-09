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

        [HttpGet("{id}")]
        public IActionResult GetSchedule(int id)
        {
            Schedule? schedule = _context.Schedules.Find(id);
            if(schedule == null)
            {
                return NotFound();
            }
            return Ok(schedule);
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

        [HttpPut("{id}")]
        public IActionResult UpdateSchedule(int id, Schedule schedule)
        {
            var mySchedule = _context.Schedules.Find(id);
            if(mySchedule == null)
            {
                return NotFound();
            }
            mySchedule.Title = schedule.Title.Trim();
            mySchedule.Description = schedule.Description.Trim();
            mySchedule.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteSchedule(int id)
        {
            var mySchedule = _context.Schedules.Find(id);
            if(mySchedule == null)
            {
                return NotFound();
            }

            _context.Schedules.Remove(mySchedule);
            _context.SaveChanges();
            return Ok();
        }
    }
}
