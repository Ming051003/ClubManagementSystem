using Model.Models;
using System.Collections.Generic;

namespace BLL.BusinessInterfaces
{
    public interface IEventParticipantService
    {
        List<EventParticipant> GetAllEventParticipants();
        EventParticipant GetEventParticipantById(int id);
        List<EventParticipant> GetEventParticipantsByEvent(int eventId);
        List<EventParticipant> GetEventParticipantsByUser(int userId);
        void AddEventParticipant(EventParticipant participant);
        void UpdateEventParticipant(EventParticipant participant);
        void DeleteEventParticipant(int id);
        void DeleteEventParticipant(int eventId, int userId);
    }
}
