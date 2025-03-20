using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BLL.BusinessInterfaces;
using BLL.BusinessService;
using DAL.Interfaces;
using DAL.Repositories;
using Model.Contexts;
using WPF.ViewModels;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MemberManagement.xaml
    /// </summary>
    public partial class MemberManagement : Window
    {
        public MemberManagement()
        {
            InitializeComponent();
            
            // Create context and repositories
            var context = new ClubManagementContext();
            IUserRepository userRepository = new UserRepository(context);
            IEventParticipantRepository eventParticipantRepository = new EventParticipantRepository(context);
            
            // Create services
            IUserService userService = new UserService(userRepository);
            IEventParticipantService eventParticipantService = new EventParticipantService(eventParticipantRepository);
            
            // Create and set ViewModel
            DataContext = new MemberViewModel(userService, eventParticipantService);
        }
    }
}
