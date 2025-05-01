using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Курсач_1.Controllers;
using Курсач_1.Models;

namespace Курсач_1.Repositories
{
    public class EventsRepositories
    {
        private readonly ApplicationDbContext _dbContext;
        public EventsRepositories(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<EventDemonstration>> GetList(int id)
        {
            var listEvents =  _dbContext.Events.Where(x => x.UserId == id).ToList();
            List<EventDemonstration> listDemenstration = listEvents.Select(x => new EventDemonstration
            {
                Id = x.Id,
                Description = x.Description,
                Time = DateTime.Parse(x.Time.ToShortDateString())//DateTime.Parse(x.Time.ToString("dd.MM.yyyy"))
            }).ToList();
            return listDemenstration;
        }

        public async Task Add(int id, string description, DateTime date)
        {
            Event newEvent = new Event
            {
                UserId = id,
                Description = description,
                Time = date
            };

            _dbContext.Events.Add(newEvent);
            await _dbContext.SaveChangesAsync(); // Сохраняем изменения в базе данных
        }

        public async Task Update(int eventId, CreateEventRequest eventRequest)
        {
            Event updateEvent = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);

            if (updateEvent == null)
                throw new Exception("Событие не найдено");

            updateEvent.Description = eventRequest.Description == "string" ? updateEvent.Description : eventRequest.Description;
            updateEvent.Time = eventRequest.Time.ToString() == "2025-04-29T15:24:49.909Z" ? updateEvent.Time : eventRequest.Time;

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
        public DateTime Time { get; set; }
    }
}
