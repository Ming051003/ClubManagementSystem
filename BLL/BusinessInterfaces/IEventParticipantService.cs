using Model.Models;
using System.Collections.Generic;

namespace BLL.BusinessInterfaces
{
    public interface IEventParticipantService
    {
        public List<EventParticipant> GetAllEventParticipants();
        public EventParticipant GetEventParticipantById(int id);
        public List<EventParticipant> GetEventParticipantsByEvent(int eventId);
        public List<EventParticipant> GetEventParticipantsByUser(int userId);
        public void AddEventParticipant(EventParticipant participant);
        public void UpdateEventParticipant(EventParticipant participant);
        public void DeleteEventParticipant(int id);
        public void DeleteEventParticipant(int eventId, int userId);
    }
}
