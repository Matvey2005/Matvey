using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text.Json.Serialization;
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
        public async Task<ActionResult<List<EventDemonstration>>> GetListEvents(
            [FromQuery] int? page = null,
            [FromQuery] int? pageSize = null,
            [FromQuery] DateTimeOffset? from = null,
            [FromQuery] DateTimeOffset? to = null,
            [FromQuery] string? sort = null
        )
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = idClaim != null ? int.Parse(idClaim.Value) : (int?)null;

            if (userId == null) return Unauthorized();

            var result = await repositories.GetList(userId.Value, page, pageSize, from, to, sort);
            return Ok(result);
        }



        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Event>> AddEvent([FromBody] CreateEventRequest newEvent)
        {

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = idClaim != null ? int.Parse(idClaim.Value) : (int?)null;
            var ev = await repositories.Add(userId.Value, newEvent.Description, newEvent.Time);
            return Ok(ev);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult> UpdateEvent([FromBody] CreateEventRequest eventRequest, int id)
        {
            var ev = await repositories.UpdateAll(id, eventRequest);
            return Ok(ev);
        }

        [HttpPatch("{id:int}")]
        [Authorize]
        public async Task<ActionResult> UpdatePartially(int id, [FromBody] PatchEventRequest patchEventRequest)
        {
            await repositories.UpdatePatch(id, patchEventRequest);
            return Ok("Обновлено успешно");
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<ActionResult> DeleteEvent(int id)
        {
            await repositories.Delete(id);
            return Ok("Удаление прошло успешно");
        }

        [HttpGet("test")]
        public async Task<ActionResult<List<EventDemonstration>>> GetTest()
        {
            Console.WriteLine("GET /test was called");
            var result = await repositories.GetList(5, 1, 10, null, null, "desc");
            return Ok(result);
        }
    }

    public class CreateEventRequest
    {
        public string Description { get; set; }
        public DateTimeOffset Time { get; set; }
    }

    public class PatchEventRequest
    {
        public string? Description { get; set; } = null;

        public DateTimeOffset? Time { get; set; }
    }

}
