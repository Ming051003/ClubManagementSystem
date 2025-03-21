using BLL.BusinessInterfaces;
using BLL.BusinessService;
using Microsoft.Extensions.DependencyInjection;
using Model.Models;
using System.Windows;
using System.Windows.Controls;

namespace WPF.Admin
{
    /// <summary>
    /// Interaction logic for ClubManagement.xaml
    /// </summary>
    public partial class ClubManagement : UserControl
    {
        private readonly IClubService clubService;
        public ClubManagement()
        {
            InitializeComponent();
            clubService = ((App)Application.Current).ServiceProvider.GetRequiredService<IClubService>()
              ?? throw new ArgumentNullException(nameof(ClubService));
            LoadData();
        }

        private void LoadData(string searchText = "")
        {
            var data = clubService.GetClubs()
                .Where(c => string.IsNullOrEmpty(searchText) || c.ClubName.Contains(searchText))
                .Select(a => new Club
                {
                    ClubId = a.ClubId,
                    ClubName = a.ClubName,
                    Description = a.Description,
                    EstablishedDate = a.EstablishedDate
                })
                .ToList();
            dgClub.ItemsSource = data;
        }


        private void dgClub_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedClub = dgClub.SelectedItem as Club;
            if (selectedClub != null)
            {
                txtClubId.Text = selectedClub.ClubId.ToString();
                txtClubName.Text = selectedClub.ClubName;
                txtDescription.Text = selectedClub.Description;
                dpEstablishedDate.SelectedDate = selectedClub.EstablishedDate?.ToDateTime(new TimeOnly(0, 0));
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //int clubId;
                //if (!int.TryParse(txtClubId.Text, out clubId))
                //{
                //    MessageBox.Show("Please enter a valid Club ID.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                //    return;
                //}

                string clubName = txtClubName.Text.Trim();
                string description = txtDescription.Text.Trim();

                if (clubService.GetClubs().Any(c => c.ClubName == clubName))
                {
                    MessageBox.Show("Club Name already exists. Please enter another one.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!dpEstablishedDate.SelectedDate.HasValue)
                {
                    MessageBox.Show("Please select the establishment date.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Club club = new Club
                {
                    //ClubId = clubId,  
                    ClubName = clubName,
                    Description = description,
                    EstablishedDate = DateOnly.FromDateTime(dpEstablishedDate.SelectedDate.Value) // Chuyển đổi từ DateTime sang DateOnly
                };

                clubService.AddClub(club);

                MessageBox.Show("Club added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedClub = dgClub.SelectedValue as Club;
            if (selectedClub == null) return;
            if (!ValidateForm())
            {
                return;
            }

            var updatedClub = new Club
            {
                ClubId = selectedClub.ClubId,
                ClubName = txtClubName.Text,
                Description =txtDescription.Text,
                EstablishedDate = DateOnly.FromDateTime(dpEstablishedDate.SelectedDate.Value)
            };
            try
            {
                clubService.UpdateClub(updatedClub);
                MessageBox.Show("Club updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
                ClearForm();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var selectedClub = dgClub.SelectedValue as Club;
            if (selectedClub == null)
            {
                MessageBox.Show("Please select a user to delete!", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var result = MessageBox.Show("Are you sure you want to delete this user?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    clubService.DeleteClub(selectedClub.ClubId);
                    MessageBox.Show("User deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                    ClearForm();

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            string searchText = tbSearch.Text.Trim();
            LoadData(searchText);
            ClearFilter();
        }

        private void ClearFilter()
        {
            tbSearch.Text = "";
        }
        private void ClearForm()
        {
            txtClubId.Text = string.Empty;
            txtClubName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            dpEstablishedDate.SelectedDate = null;
        }
        private bool ValidateForm()
        {
            //int clubId;
            string clubName = txtClubName.Text.Trim();
            string description = txtDescription.Text.Trim();
            //if (!int.TryParse(txtClubId.Text, out clubId))
            //{
            //    MessageBox.Show("Please enter a valid Club ID.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return false;
            //}
                   
            if (clubService.GetClubs().Any(c => c.ClubName == clubName))
            {
                MessageBox.Show("Club Name already exists. Please enter another one.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Kiểm tra xem người dùng đã chọn ngày thành lập chưa
            if (!dpEstablishedDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select the establishment date.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
    }
}
