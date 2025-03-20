using Model.Models;
using System.Collections.Generic;

namespace DAL.Interfaces
{
    public interface IEventRepository
    {
        List<Event> GetAll();
        Event GetById(int id);
        void Add(Event entity);
        void Update(Event entity);
        void Delete(int id);
        List<Event> GetByClubId(int clubId);
        List<Event> GetUpcomingEvents();
        List<Event> GetPastEvents();
    }
}
