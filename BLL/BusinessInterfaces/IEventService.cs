using Model.Models;
using System.Collections.Generic;

namespace BLL.BusinessInterfaces
{
    public interface IEventService
    {
        public List<Event> GetAllEvents();
        public Event GetEventById(int id);
        public List<Event> GetEventsByClub(int clubId);
        public List<Event> GetEventsByStatus(string status);
        public void AddEvent(Event eventObj);
        public void UpdateEvent(Event eventObj);
        public void DeleteEvent(int id);
    }
}
