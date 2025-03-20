using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Contexts;
using Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repositories
{
    public class EventParticipantRepository : IEventParticipantRepository
    {
        private readonly ClubManagementContext _context;

        public EventParticipantRepository(ClubManagementContext context)
        {
            _context = context;
        }

        public List<EventParticipant> GetAll()
        {
            return _context.EventParticipants
                .Include(ep => ep.Event)
                .Include(ep => ep.User)
                .ToList();
        }

        public EventParticipant GetById(int id)
        {
            return _context.EventParticipants
                .Include(ep => ep.Event)
                .Include(ep => ep.User)
                .FirstOrDefault(ep => ep.EventParticipantId == id);
        }

        public List<EventParticipant> GetByEventId(int eventId)
        {
            return _context.EventParticipants
                .Include(ep => ep.User)
                .Where(ep => ep.EventId == eventId)
                .ToList();
        }

        public List<EventParticipant> GetByUserId(int userId)
        {
            return _context.EventParticipants
                .Include(ep => ep.Event)
                .Where(ep => ep.UserId == userId)
                .ToList();
        }

        public void Add(EventParticipant entity)
        {
            _context.EventParticipants.Add(entity);
            _context.SaveChanges();
        }

        public void Update(EventParticipant entity)
        {
            _context.EventParticipants.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = _context.EventParticipants.Find(id);
            if (entity != null)
            {
                _context.EventParticipants.Remove(entity);
                _context.SaveChanges();
            }
        }
    }
}
