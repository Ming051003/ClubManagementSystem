using BLL.BusinessInterfaces;
using DAL.Interfaces;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.BusinessService
{
    public class EventParticipantService : IEventParticipantService
    {
        private readonly IEventParticipantRepository _eventParticipantRepository;

        public EventParticipantService(IEventParticipantRepository eventParticipantRepository)
        {
            _eventParticipantRepository = eventParticipantRepository;
        }

        public IEnumerable<EventParticipant> GetAllEventParticipants()
        {
            return _eventParticipantRepository.GetAll();
        }

        public EventParticipant GetEventParticipantById(int id)
        {
            return _eventParticipantRepository.GetById(id);
        }

        public IEnumerable<EventParticipant> GetEventParticipantsByEvent(int eventId)
        {
            return _eventParticipantRepository.GetAll().Where(p => p.EventId == eventId);
        }

        public IEnumerable<EventParticipant> GetEventParticipantsByUser(int userId)
        {
            return _eventParticipantRepository.GetAll().Where(p => p.UserId == userId);
        }

        public void AddEventParticipant(EventParticipant participant)
        {
            // Validate participant data
            if (participant.EventId <= 0)
                throw new ArgumentException("Event ID is required");

            if (participant.UserId <= 0)
                throw new ArgumentException("User ID is required");

            if (string.IsNullOrEmpty(participant.Status))
                participant.Status = "Registered";

            if (participant.RegistrationDate == DateTime.MinValue)
                participant.RegistrationDate = DateTime.Now;

            // Check if the user is already registered for this event
            var existingParticipant = _eventParticipantRepository.GetAll()
                .FirstOrDefault(p => p.EventId == participant.EventId && p.UserId == participant.UserId);

            if (existingParticipant != null)
                throw new ArgumentException("User is already registered for this event");

            _eventParticipantRepository.Add(participant);
        }

        public void UpdateEventParticipant(EventParticipant participant)
        {
            // Validate participant data
            if (participant.EventId <= 0)
                throw new ArgumentException("Event ID is required");

            if (participant.UserId <= 0)
                throw new ArgumentException("User ID is required");

            if (string.IsNullOrEmpty(participant.Status))
                throw new ArgumentException("Status is required");

            // Validate status
            if (!new[] { "Registered", "Attended", "Absent" }.Contains(participant.Status))
                throw new ArgumentException("Invalid status value");

            _eventParticipantRepository.Update(participant);
        }

        public void DeleteEventParticipant(int id)
        {
            _eventParticipantRepository.Delete(id);
        }
        
        public void DeleteEventParticipant(int eventId, int userId)
        {
            // Find the participant with the specified eventId and userId
            var participant = _eventParticipantRepository.GetAll()
                .FirstOrDefault(p => p.EventId == eventId && p.UserId == userId);
                
            if (participant == null)
                throw new ArgumentException($"Participant not found for Event ID {eventId} and User ID {userId}");
                
            // Delete the participant using the existing method
            _eventParticipantRepository.Delete(participant.EventParticipantId);
        }
    }
}
