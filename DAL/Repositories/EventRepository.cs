using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Contexts;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ClubManagementContext _context;

        public EventRepository()
        {
            _context = new();
        }

        public List<Event> GetAll()
        {
            return _context.Events
                .Include(e => e.Club)
                .ToList();
        }

        public Event GetById(int id)
        {
            return _context.Events
                .Include(e => e.Club)
                .FirstOrDefault(e => e.EventId == id);
        }

        public List<Event> GetByClubId(int clubId)
        {
            return _context.Events
                .Include(e => e.Club)
                .Where(e => e.ClubId == clubId)
                .ToList();
        }

        public List<Event> GetUpcomingEvents()
        {
            var currentDate = DateTime.Now;
            return _context.Events
                .Include(e => e.Club)
                .Where(e => e.EventDate > currentDate)
                .OrderBy(e => e.EventDate)
                .ToList();
        }

        public List<Event> GetPastEvents()
        {
            var currentDate = DateTime.Now;
            return _context.Events
                .Include(e => e.Club)
                .Where(e => e.EventDate < currentDate)
                .OrderByDescending(e => e.EventDate)
                .ToList();
        }

        public void Add(Event entity)
        {
            _context.Events.Add(entity);
            _context.SaveChanges();
        }

        public void Update(Event entity)
        {
            _context.Events.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = _context.Events.Find(id);
            if (entity != null)
            {
                _context.Events.Remove(entity);
                _context.SaveChanges();
            }
        }
    }
}
