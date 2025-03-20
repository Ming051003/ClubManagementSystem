using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Contexts;
using Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repositories
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        public EventRepository(ClubManagementContext context) : base(context)
        {
        }

        public override IEnumerable<Event> GetAll()
        {
            return _context.Events
                .Include(e => e.Club)
                .ToList();
        }

        public override Event GetById(int id)
        {
            return _context.Events
                .Include(e => e.Club)
                .FirstOrDefault(e => e.EventId == id);
        }
    }
}
