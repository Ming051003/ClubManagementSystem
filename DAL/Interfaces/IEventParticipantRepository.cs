using Model.Models;
using System.Collections.Generic;

namespace DAL.Interfaces
{
    public interface IEventParticipantRepository
    {
        public List<EventParticipant> GetAll();
        public EventParticipant GetById(int id);
        public void Add(EventParticipant entity);
        public void Update(EventParticipant entity);
        public void Delete(int id);
        public List<EventParticipant> GetByEventId(int eventId);
        public List<EventParticipant> GetByUserId(int userId);
    }
}
