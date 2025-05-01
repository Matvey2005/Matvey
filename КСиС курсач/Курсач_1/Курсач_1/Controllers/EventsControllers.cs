using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Security.Claims;
using Курсач_1.Models;
using Курсач_1.Repositories;

namespace Курсач_1.Controllers
{
    [ApiController]
    [Route("/events")]
    public class EventsControllers(EventsRepositories repositories) : ControllerBase
    {

        [HttpGet]
        [Authorize]
        public async Task<List<EventDemonstration>> GetListEvents()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = idClaim != null ? int.Parse(idClaim.Value) : (int?)null;
            return  await repositories.GetList(userId.Value);            
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> AddEvent([FromBody] CreateEventRequest newEvent)
        {

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = idClaim != null ? int.Parse(idClaim.Value) : (int?)null;
            await repositories.Add(userId.Value, newEvent.Description, newEvent.Time);
            return Created();
        }

        [HttpPatch("{id:int}")]
        [Authorize]
        public async Task<ActionResult> UpdateEvent([FromBody] CreateEventRequest eventRequest, int id)
        {
            await repositories.Update(id, eventRequest);
            return Ok("Обновлено успешно");
        }

        [HttpDelete("/{id:int}")]
        [Authorize]
        public async Task<ActionResult> DeleteEvent(int eventId)
        {
            await repositories.Delete(eventId);
            return Ok("Удаление прошло успешно");
        }
    }

    public class CreateEventRequest
    {
        public string Description { get; set; }
        public DateTime Time { get; set; }
    }

}
