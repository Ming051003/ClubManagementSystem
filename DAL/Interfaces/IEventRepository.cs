using Model.Models;
using System.Collections.Generic;

namespace DAL.Interfaces
{
    public interface IEventRepository
    {
        public List<Event> GetAll();
        public Event GetById(int id);
        public void Add(Event entity);
        public void Update(Event entity);
        public void Delete(int id);
        public List<Event> GetByClubId(int clubId);
        public List<Event> GetUpcomingEvents();
        public List<Event> GetPastEvents();
    }
}
