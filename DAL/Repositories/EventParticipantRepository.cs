using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Contexts;
using Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repositories
{
    public class EventParticipantRepository : Repository<EventParticipant>, IEventParticipantRepository
    {
        public EventParticipantRepository(ClubManagementContext context) : base(context)
        {
        }

        public override IEnumerable<EventParticipant> GetAll()
        {
            return _context.EventParticipants
                .Include(ep => ep.Event)
                .Include(ep => ep.User)
                .ToList();
        }

        public override EventParticipant GetById(int id)
        {
            return _context.EventParticipants
                .Include(ep => ep.Event)
                .Include(ep => ep.User)
                .FirstOrDefault(ep => ep.EventParticipantId == id);
        }
    }
}
