using Model.Models;
using System.Collections.Generic;

namespace BLL.BusinessInterfaces
{
    public interface IEventParticipantService
    {
        IEnumerable<EventParticipant> GetAllEventParticipants();
        EventParticipant GetEventParticipantById(int id);
        IEnumerable<EventParticipant> GetEventParticipantsByEvent(int eventId);
        IEnumerable<EventParticipant> GetEventParticipantsByUser(int userId);
        void AddEventParticipant(EventParticipant participant);
        void UpdateEventParticipant(EventParticipant participant);
        void DeleteEventParticipant(int id);
        void DeleteEventParticipant(int eventId, int userId);
    }
}
