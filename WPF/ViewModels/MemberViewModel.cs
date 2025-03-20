using Model.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.BusinessInterfaces;
using BLL.BusinessService;

namespace WPF.ViewModels
{
    public class MemberViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly IEventParticipantService _eventParticipantService;
        
        private ObservableCollection<User> _members;
        private User _selectedMember;
        private string _searchText;
        private string _searchCriteria;
        private ObservableCollection<EventParticipant> _memberActivities;
        private bool _isEditMode;
        private User _editingMember;

        public MemberViewModel(IUserService userService, IEventParticipantService eventParticipantService)
        {
            _userService = userService;
            _eventParticipantService = eventParticipantService;
            
            // Initialize collections
            Members = new ObservableCollection<User>();
            MemberActivities = new ObservableCollection<EventParticipant>();
            SearchCriteria = "FullName"; // Default search criteria
            
            // Initialize commands
            AddMemberCommand = new RelayCommand(param => AddMember());
            EditMemberCommand = new RelayCommand(param => EditMember(), param => SelectedMember != null);
            DeleteMemberCommand = new RelayCommand(param => DeleteMember(), param => SelectedMember != null);
            SaveMemberCommand = new RelayCommand(param => SaveMember(), param => CanSaveMember());
            CancelEditCommand = new RelayCommand(param => CancelEdit());
            SearchCommand = new RelayCommand(param => Search());
            ViewMemberActivitiesCommand = new RelayCommand(param => ViewMemberActivities(), param => SelectedMember != null);
            
            // Load data
            LoadMembers();
        }

        public ObservableCollection<User> Members
        {
            get => _members;
            set => SetProperty(ref _members, value);
        }

        public User SelectedMember
        {
            get => _selectedMember;
            set
            {
                if (SetProperty(ref _selectedMember, value) && value != null)
                {
                    // Create a copy for editing to avoid direct modification
                    _editingMember = new User
                    {
                        UserId = value.UserId,
                        StudentId = value.StudentId,
                        FullName = value.FullName,
                        Email = value.Email,
                        Password = value.Password,
                        Role = value.Role,
                        ClubId = value.ClubId,
                        JoinDate = value.JoinDate,
                        Status = value.Status
                    };
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public string SearchCriteria
        {
            get => _searchCriteria;
            set => SetProperty(ref _searchCriteria, value);
        }

        public ObservableCollection<EventParticipant> MemberActivities
        {
            get => _memberActivities;
            set => SetProperty(ref _memberActivities, value);
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public User EditingMember
        {
            get => _editingMember;
            set => SetProperty(ref _editingMember, value);
        }

        public ICommand AddMemberCommand { get; }
        public ICommand EditMemberCommand { get; }
        public ICommand DeleteMemberCommand { get; }
        public ICommand SaveMemberCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ViewMemberActivitiesCommand { get; }

        private async void LoadMembers()
        {
            try
            {
                var members = await Task.Run(() => _userService.GetAllUsers());
                Members.Clear();
                foreach (var member in members)
                {
                    Members.Add(member);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading members: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddMember()
        {
            IsEditMode = true;
            EditingMember = new User
            {
                JoinDate = DateOnly.FromDateTime(DateTime.Now),
                Status = true // Active by default  
            };
        }

        private void EditMember()
        {
            if (SelectedMember != null)
            {
                IsEditMode = true;
            }
        }

        private async void DeleteMember()
        {
            if (SelectedMember == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete {SelectedMember.FullName}?", 
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await Task.Run(() => _userService.DeleteUser(SelectedMember.UserId));
                    Members.Remove(SelectedMember);
                    MessageBox.Show("Member deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting member: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void SaveMember()
        {
            if (EditingMember == null || !CanSaveMember()) return;

            try
            {
                if (EditingMember.UserId == 0) // New member
                {
                    await Task.Run(() => _userService.AddUser(EditingMember));
                    Members.Add(EditingMember);
                    MessageBox.Show("Member added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else // Existing member
                {
                    await Task.Run(() => _userService.UpdateUser(EditingMember));
                    
                    // Update the item in the collection
                    var index = Members.IndexOf(SelectedMember);
                    if (index >= 0)
                    {
                        Members[index] = EditingMember;
                    }
                    
                    MessageBox.Show("Member updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                IsEditMode = false;
                SelectedMember = EditingMember;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving member: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelEdit()
        {
            IsEditMode = false;
            EditingMember = null;
        }

        private bool CanSaveMember()
        {
            if (EditingMember == null) return false;
            
            // Basic validation
            return !string.IsNullOrWhiteSpace(EditingMember.FullName) &&
                   !string.IsNullOrWhiteSpace(EditingMember.Email) &&
                   !string.IsNullOrWhiteSpace(EditingMember.Password) &&
                   !string.IsNullOrWhiteSpace(EditingMember.Role);
        }

        private void Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadMembers();
                return;
            }

            try
            {
                IEnumerable<User> filteredMembers;
                
                switch (SearchCriteria)
                {
                    case "StudentID":
                        filteredMembers = _userService.GetAllUsers()
                            .Where(m => m.StudentId != null && m.StudentId.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "Role":
                        filteredMembers = _userService.GetAllUsers()
                            .Where(m => m.Role.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "FullName":
                    default:
                        filteredMembers = _userService.GetAllUsers()
                            .Where(m => m.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
                        break;
                }

                Members.Clear();
                foreach (var member in filteredMembers)
                {
                    Members.Add(member);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching members: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ViewMemberActivities()
        {
            if (SelectedMember == null) return;

            try
            {
                var activities = await Task.Run(() => 
                    _eventParticipantService.GetEventParticipantsByUser(SelectedMember.UserId));
                
                MemberActivities.Clear();
                foreach (var activity in activities)
                {
                    MemberActivities.Add(activity);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading member activities: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
