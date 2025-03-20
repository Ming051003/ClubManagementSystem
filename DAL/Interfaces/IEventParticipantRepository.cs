using Model.Models;
using System.Collections.Generic;

namespace DAL.Interfaces
{
    public interface IEventParticipantRepository
    {
        List<EventParticipant> GetAll();
        EventParticipant GetById(int id);
        void Add(EventParticipant entity);
        void Update(EventParticipant entity);
        void Delete(int id);
        List<EventParticipant> GetByEventId(int eventId);
        List<EventParticipant> GetByUserId(int userId);
    }
}
