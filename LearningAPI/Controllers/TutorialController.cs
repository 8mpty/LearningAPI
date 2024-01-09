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
    public class TutorialController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TutorialController> _logger;
        public TutorialController(MyDbContext context, IMapper mapper, ILogger<TutorialController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }
        // private static readonly List<Tutorial> list = new();

        private int GetUserId()
        {
            return Convert.ToInt32(User.Claims
            .Where(c => c.Type == ClaimTypes.NameIdentifier)
            .Select(c => c.Value).SingleOrDefault());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TutorialDTO), StatusCodes.Status200OK)]
        public IActionResult GetTutorial(int id)
        {
            try
            {
                Tutorial? tutorial = _context.Tutorials.Include(t => t.User).FirstOrDefault(t => t.Id == id);
                if (tutorial == null)
                {
                    return NotFound();
                }
                TutorialDTO data = _mapper.Map<TutorialDTO>(tutorial);
                /** var data = new
                {
                    tutorial.Id,
                    tutorial.Title,
                    tutorial.Description,
                    tutorial.ImageFile,
                    tutorial.CreatedAt,
                    tutorial.UpdatedAt,
                    tutorial.UserId,
                    User = new
                    {
                        tutorial.User?.Name
                    }
                }; **/
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting tutorial");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TutorialDTO>), StatusCodes.Status200OK)]
        public IActionResult GetAll(string? search)
        {
            try
            {
                IQueryable<Tutorial> result = _context.Tutorials.Include(t => t.User);
                if (search != null)
                {
                    result = result.Where(x => x.Title.Contains(search)
                    || x.Description.Contains(search));
                }
                var list = result.OrderByDescending(x => x.CreatedAt).ToList();
                IEnumerable<TutorialDTO> data = list.Select(t => _mapper.Map<TutorialDTO>(t));
                /** var data = list.Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Description,
                    t.ImageFile,
                    t.CreatedAt,
                    t.UpdatedAt,
                    t.UserId,
                    User = new
                    {
                        t.User?.Name
                    }
                }); **/
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when get all tutorials");
                return StatusCode(500);
            }
        }

        [HttpPost, Authorize]
        [ProducesResponseType(typeof(TutorialDTO), StatusCodes.Status200OK)]
        public IActionResult AddTutorial(AddTutorialRequest tutorial)
        {
            try
            {
                int userId = GetUserId();
                var now = DateTime.Now;
                var myTutorial = new Tutorial()
                {
                    Title = tutorial.Title.Trim(),
                    Description = tutorial.Description.Trim(),
                    ImageFile = tutorial.ImageFile,
                    CreatedAt = now,
                    UpdatedAt = now,
                    UserId = userId
                };
                _context.Tutorials.Add(myTutorial);
                _context.SaveChanges();
                Tutorial? newTutorial = _context.Tutorials.Include(t => t.User)
                .FirstOrDefault(t => t.Id == myTutorial.Id);
                TutorialDTO tutorialDTO = _mapper.Map<TutorialDTO>(newTutorial);
                return Ok(tutorialDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when adding tutorial");
                return StatusCode(500);
            }
        }

        [HttpPut("{id}"), Authorize]
        public IActionResult UpdateTutorial(int id, UpdateTutorialRequest tutorial)
        {
            try
            {
                var myTutorial = _context.Tutorials.Find(id);
                if (myTutorial == null)
                {
                    return NotFound();
                }
                int userId = GetUserId();
                if (myTutorial.UserId != userId)
                {
                    return Forbid();
                }
                if (tutorial.Title != null)
                {
                    myTutorial.Title = tutorial.Title.Trim();
                }
                if (tutorial.Description != null)
                {
                    myTutorial.Description = tutorial.Description.Trim();
                }
                if (tutorial.ImageFile != null)
                {
                    myTutorial.ImageFile = tutorial.ImageFile;
                }
                myTutorial.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when updating tutorial");
                return StatusCode(500);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTutorial(int id)
        {
            try
            {
                var myTutorial = _context.Tutorials.Find(id);
                if (myTutorial == null)
                {
                    return NotFound();
                }
                int userId = GetUserId();
                if (myTutorial.UserId != userId)
                {
                    return Forbid();
                }
                _context.Tutorials.Remove(myTutorial);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when deleting tutorial");
                return StatusCode(500);
            }
        }
    }
}
