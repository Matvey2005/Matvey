using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Text.Json.Serialization;
using Курсач_1.Controllers;
using Курсач_1.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Курсач_1.Repositories
{
    public class EventsRepositories
    {
        private readonly ApplicationDbContext _dbContext;
        public EventsRepositories(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<EventDemonstration>> GetList(
            int userId,
            int? page,
            int? pageSize,
            DateTimeOffset? from,
            DateTimeOffset? to,
            string? sort
        )
        {
            var query = _dbContext.Events.Where(x => x.UserId == userId);
            Console.WriteLine(query.ToString());
            if (from.HasValue)
            {
                var fromDate = from.Value.ToUniversalTime();
                query = query.Where(x => x.Time >= fromDate);
            }

            if (to.HasValue)
            {
                var toDate = to.Value.ToUniversalTime();
                query = query.Where(x => x.Time <= toDate);
            }

            // Применяем сортировку только если явно указана
            if (!string.IsNullOrWhiteSpace(sort))
            {
                query = sort.ToLower() switch
                {
                    "asc" => query.OrderBy(e => e.Time),
                    "desc" => query.OrderByDescending(e => e.Time),
                    _ => query // игнорируем сортировку, если значение некорректное
                };
            }

            // Применяем пагинацию только если указаны оба параметра
            if (page.HasValue && pageSize.HasValue)
            {
                query = query
                    .Skip((page.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            }

            var items = await query.ToListAsync();

            return items.Select(x => new EventDemonstration
            {
                Id = x.Id,
                Description = x.Description,
                Time = x.Time
            }).ToList();
        }




        public async Task<Event> Add(int id, string description, DateTimeOffset date)
        {
            Event newEvent = new Event
            {
                UserId = id,
                Description = description,
                Time = date.ToUniversalTime(),
            };

            _dbContext.Events.Add(newEvent);
            await _dbContext.SaveChangesAsync(); // Сохраняем изменения в базе данных
            return newEvent;
        }

        public async Task<Event> UpdateAll(int eventId, CreateEventRequest eventRequest)
        {
            Event updateEvent = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);

            if (updateEvent == null)
                throw new Exception("Событие не найдено");

            updateEvent.Description = eventRequest.Description;
            updateEvent.Time = eventRequest.Time;

            await _dbContext.SaveChangesAsync();
            return updateEvent;
        }

        public async Task UpdatePatch(int eventId, PatchEventRequest patchEventRequest)
        {
            Event updateEvent = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);

            if (updateEvent == null)
                throw new Exception("Событие не найдено");

            if (patchEventRequest.Description != null)
            {
                updateEvent.Description = patchEventRequest.Description;
            }

            if (patchEventRequest.Time.HasValue)
            {
                updateEvent.Time = patchEventRequest.Time.Value;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(int evId)
        {
            var eventToRemove = await _dbContext.Events
                .FirstOrDefaultAsync(x => x.Id == evId);

            if (eventToRemove != null)
            {
                _dbContext.Events.Remove(eventToRemove);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Событие не найдено.");
            }

        }

    }

    public class EventDemonstration
    {     
        public int Id { get; set; }
        public string Description { get; set; }

        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTimeOffset Time { get; set; }
    }
}
