using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BLL.BusinessInterfaces;
using BLL.BusinessService;
using DAL.Interfaces;
using DAL.Repositories;
using Model.Contexts;
using Model.Models;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MemberManagement.xaml
    /// </summary>
    public partial class MemberManagement : Window
    {
        private readonly IUserService _userService;
        private readonly IEventParticipantService _eventParticipantService;
        
        private List<User> _members;
        private User _selectedMember;
        private User _editingMember;
        private List<EventParticipant> _memberActivities;

        public MemberManagement()
        {
            InitializeComponent();
            
            // Create context and repositories
            var context = new ClubManagementContext();
            IUserRepository userRepository = new UserRepository(context);
            IEventParticipantRepository eventParticipantRepository = new EventParticipantRepository(context);
            
            // Create services
            _userService = new UserService(userRepository);
            _eventParticipantService = new EventParticipantService(eventParticipantRepository);
            
            // Load members
            LoadMembers();
        }

        private void LoadMembers()
        {
            try
            {
                _members = _userService.GetAllUsers().ToList();
                dgMembers.ItemsSource = _members;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading members: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                LoadMembers();
                return;
            }

            try
            {
                IEnumerable<User> filteredMembers;
                string searchCriteria = ((ComboBoxItem)cmbSearchCriteria.SelectedItem).Content.ToString();
                
                switch (searchCriteria)
                {
                    case "StudentID":
                        filteredMembers = _userService.GetAllUsers()
                            .Where(m => m.StudentId != null && m.StudentId.Contains(searchText, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "Role":
                        filteredMembers = _userService.GetAllUsers()
                            .Where(m => m.Role.Contains(searchText, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "FullName":
                    default:
                        filteredMembers = _userService.GetAllUsers()
                            .Where(m => m.FullName.Contains(searchText, StringComparison.OrdinalIgnoreCase));
                        break;
                }

                dgMembers.ItemsSource = filteredMembers.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching members: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgMembers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedMember = dgMembers.SelectedItem as User;
            
            if (_selectedMember != null)
            {
                // Create a copy for editing to avoid direct modification
                _editingMember = new User
                {
                    UserId = _selectedMember.UserId,
                    StudentId = _selectedMember.StudentId,
                    FullName = _selectedMember.FullName,
                    Email = _selectedMember.Email,
                    Password = _selectedMember.Password,
                    Role = _selectedMember.Role,
                    ClubId = _selectedMember.ClubId,
                    JoinDate = _selectedMember.JoinDate,
                    Status = _selectedMember.Status
                };
            }
        }

        private void btnAddMember_Click(object sender, RoutedEventArgs e)
        {
            ShowEditForm(true);
            
            _editingMember = new User
            {
                JoinDate = DateOnly.FromDateTime(DateTime.Now),
                Status = true // Active by default
            };
            
            // Clear form fields
            txtStudentId.Text = string.Empty;
            txtFullName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtPassword.Password = string.Empty;
            cmbRole.SelectedIndex = 0; // Default to Member
            txtClubId.Text = string.Empty;
            dpJoinDate.SelectedDate = DateTime.Now;
            chkStatus.IsChecked = true;
        }

        private void btnEditMember_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedMember == null)
            {
                MessageBox.Show("Please select a member to edit.", "No Member Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            
            ShowEditForm(true);
            
            // Populate form fields
            txtStudentId.Text = _editingMember.StudentId;
            txtFullName.Text = _editingMember.FullName;
            txtEmail.Text = _editingMember.Email;
            txtPassword.Password = _editingMember.Password;
            
            // Set role
            foreach (ComboBoxItem item in cmbRole.Items)
            {
                if (item.Content.ToString() == _editingMember.Role)
                {
                    cmbRole.SelectedItem = item;
                    break;
                }
            }
            
            txtClubId.Text = _editingMember.ClubId.ToString();
            dpJoinDate.SelectedDate = _editingMember.JoinDate.Value.ToDateTime(TimeOnly.MinValue);

            chkStatus.IsChecked = _editingMember.Status;
        }

        private void btnDeleteMember_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedMember == null)
            {
                MessageBox.Show("Please select a member to delete.", "No Member Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete {_selectedMember.FullName}?", 
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _userService.DeleteUser(_selectedMember.UserId);
                    LoadMembers();
                    MessageBox.Show("Member deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting member: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnViewActivities_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedMember == null)
            {
                MessageBox.Show("Please select a member to view activities.", "No Member Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                _memberActivities = _eventParticipantService.GetEventParticipantsByUser(_selectedMember.UserId);
                
                if (_memberActivities.Count > 0)
                {
                    dgActivities.ItemsSource = _memberActivities;
                    dgActivities.Visibility = Visibility.Visible;
                    txtNoActivities.Visibility = Visibility.Collapsed;
                }
                else
                {
                    dgActivities.Visibility = Visibility.Collapsed;
                    txtNoActivities.Visibility = Visibility.Visible;
                }
                
                borderActivities.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading member activities: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateMemberData())
            {
                return;
            }

            try
            {
                // Update editing member with form values
                _editingMember.StudentId = txtStudentId.Text;
                _editingMember.FullName = txtFullName.Text;
                _editingMember.Email = txtEmail.Text;
                _editingMember.Password = txtPassword.Password;
                _editingMember.Role = ((ComboBoxItem)cmbRole.SelectedItem).Content.ToString();
                
                if (int.TryParse(txtClubId.Text, out int clubId))
                {
                    _editingMember.ClubId = clubId;
                }
                
                if (dpJoinDate.SelectedDate.HasValue)
                {
                    _editingMember.JoinDate = DateOnly.FromDateTime(dpJoinDate.SelectedDate.Value);
                }
                
                _editingMember.Status = chkStatus.IsChecked ?? false;

                if (_editingMember.UserId == 0) // New member
                {
                    _userService.AddUser(_editingMember);
                    MessageBox.Show("Member added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else // Existing member
                {
                    _userService.UpdateUser(_editingMember);
                    MessageBox.Show("Member updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                LoadMembers();
                ShowEditForm(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving member: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ShowEditForm(false);
        }

        private void ShowEditForm(bool show)
        {
            borderEditForm.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        private bool ValidateMemberData()
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Please enter a full name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please enter an email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Please enter a password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            
            if (cmbRole.SelectedItem == null)
            {
                MessageBox.Show("Please select a role.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            
            return true;
        }
    }
}
