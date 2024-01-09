using Microsoft.AspNetCore.Mvc;
using LearningAPI.Models;
using MySql.Data.Types;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace LearningAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TutorialController> _logger;

        public ScheduleController(MyDbContext context, IMapper mapper, ILogger<TutorialController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        //private static readonly List<Schedule> list = new();

        private int GetUserId()
        {
            return Convert.ToInt32(User.Claims
            .Where(c => c.Type == ClaimTypes.NameIdentifier)
            .Select(c => c.Value).SingleOrDefault());
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ScheduleDTO>), StatusCodes.Status200OK)]
        public IActionResult GetAll(string? search)
        {
            IQueryable<Schedule> result = _context.Schedules;
            if(search != null)
            {
                result = result.Where(x => x.Title.Contains(search) || x.Description.Contains(search));
            }
            var list = result.OrderByDescending(x => x.CreatedAt).ToList();
            IEnumerable<ScheduleDTO> data = list.Select(t => _mapper.Map<ScheduleDTO>(t));
            return Ok(list);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ScheduleDTO), StatusCodes.Status200OK)]
        public IActionResult GetSchedule(int id)
        {
            Schedule? schedule = _context.Schedules.Include(t => t.User).FirstOrDefault(t => t.Id == id);
            if(schedule == null)
            {
                return NotFound();
            }
            ScheduleDTO data = _mapper.Map<ScheduleDTO>(schedule);
            return Ok(schedule);
        }


        [HttpPost, Authorize]
        [ProducesResponseType(typeof(ScheduleDTO), StatusCodes.Status200OK)]
        public IActionResult AddSchedule(AddScheduleRequest schedule)
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

            Schedule? newSchedule = _context.Schedules.Include(t => t.User).FirstOrDefault(t => t.Id == mySchedule.Id);
            ScheduleDTO scheduleDTO = _mapper.Map<ScheduleDTO>(newSchedule);
            return Ok(schedule);
        }

        [HttpPut("{id}"), Authorize]
        public IActionResult UpdateSchedule(int id, UpdateScheduleRequest schedule)
        {
            try
            {
                var mySchedule = _context.Schedules.Find(id);
                if (mySchedule == null)
                {
                    return NotFound();
                }
                int userId = GetUserId();
                if (mySchedule.UserId != userId)
                {
                    return Forbid();
                }
                if (schedule.Title != null)
                {
                    mySchedule.Title = schedule.Title.Trim();
                }
                if (schedule.Description != null)
                {
                    mySchedule.Description = schedule.Description.Trim();
                }
                if (schedule.ImageFile != null)
                {
                    schedule.ImageFile = schedule.ImageFile;
                }
                mySchedule.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when updating schedule");
                return StatusCode(500);
            }
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
