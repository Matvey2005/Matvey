using Microsoft.AspNetCore.Mvc;
using Курсач_1.Models;
using Курсач_1.Repositories;

namespace Курсач_1.Controllers
{
    [ApiController]
    [Route("/inform")]
    public class SendInformationController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public SendInformationController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult GetListInformation([FromHeader] string apiKey = "12345678-ABCD-4321-DCBA-87654321FEDC", [FromQuery] DateTimeOffset? from = null,
            [FromQuery] DateTimeOffset? to = null)
        {
            if (apiKey == "12345678-ABCD-4321-DCBA-87654321FEDC")
            {
                var date = DateTimeOffset.UtcNow;
                List<User> users = _dbContext.Users.ToList();
                List<Event> events = _dbContext.Events.ToList();
                List<Information> list = new List<Information>();

                if (from != null)
                {
                    var tmp = from.Value.ToUniversalTime();
                    events = events.Where(x => x.Time > tmp).ToList();
                }
                if (to != null)
                {
                    var tmp = to.Value.ToUniversalTime();
                    events = events.Where(x => x.Time < tmp).ToList();
                }

                foreach (var user in users)
                {
                    var data = new Information
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        information = new List<EventDemonstration>()
                    };

                    foreach (var inform in events.Where(x => x.UserId == user.Id))
                    {
                        data.information.Add(new EventDemonstration
                        {
                            Id = inform.Id,
                            Description = inform.Description,
                            Time = inform.Time.ToUniversalTime()
                        });

                    }

                    if (data.information.Count > 0)
                    {
                        list.Add(data);
                    }

                }
                

                return Ok(list);
            }

            return Unauthorized("Вы не имеете доступ!");
        }

    }

    public class Information
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<EventDemonstration> information { get; set; }
    }
}

// 12345678-ABCD-4321-DCBA-87654321FEDC