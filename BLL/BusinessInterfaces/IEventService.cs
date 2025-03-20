using Model.Models;
using System.Collections.Generic;

namespace BLL.BusinessInterfaces
{
    public interface IEventService
    {
        IEnumerable<Event> GetAllEvents();
        Event GetEventById(int id);
        IEnumerable<Event> GetEventsByClub(int clubId);
        IEnumerable<Event> GetEventsByStatus(string status);
        void AddEvent(Event eventObj);
        void UpdateEvent(Event eventObj);
        void DeleteEvent(int id);
    }
}
