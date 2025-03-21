using BLL.BusinessInterfaces;
using DAL.Interfaces;
using DAL.Repositories;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.BusinessService
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService()
        {
            _eventRepository = new EventRepository();
        }

        public List<Event> GetAllEvents()
        {
            return _eventRepository.GetAll();
        }

        public Event GetEventById(int id)
        {
            return _eventRepository.GetById(id);
        }

        public List<Event> GetEventsByClub(int clubId)
        {
            return _eventRepository.GetAll().Where(e => e.ClubId == clubId).ToList();
        }

        public List<Event> GetEventsByStatus(string status)
        {
            return _eventRepository.GetAll().Where(e => e.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public void AddEvent(Event eventObj)
        {
            if (string.IsNullOrEmpty(eventObj.EventName))
                throw new ArgumentException("Event name is required");

            if (string.IsNullOrEmpty(eventObj.Location))
                throw new ArgumentException("Location is required");

            if (eventObj.EventDate == DateTime.MinValue)
                throw new ArgumentException("Event date is required");

            if (eventObj.ClubId <= 0)
                throw new ArgumentException("Club ID is required");

            if (string.IsNullOrEmpty(eventObj.Status))
                eventObj.Status = "Upcoming";

            _eventRepository.Add(eventObj);
        }

        public void UpdateEvent(Event eventObj)
        {
            if (string.IsNullOrEmpty(eventObj.EventName))
                throw new ArgumentException("Event name is required");

            if (string.IsNullOrEmpty(eventObj.Location))
                throw new ArgumentException("Location is required");

            if (eventObj.EventDate == DateTime.MinValue)
                throw new ArgumentException("Event date is required");

            if (eventObj.ClubId <= 0)
                throw new ArgumentException("Club ID is required");

            if (!new[] { "Upcoming", "Ongoing", "Completed", "Cancelled" }.Contains(eventObj.Status))
                throw new ArgumentException("Invalid status value");

            _eventRepository.Update(eventObj);
        }

        public void DeleteEvent(int id)
        {
            _eventRepository.Delete(id);
        }
    }
}
