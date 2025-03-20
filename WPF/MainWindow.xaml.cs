using System.Windows;

namespace WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnMemberManagement_Click(object sender, RoutedEventArgs e)
        {
            var memberManagement = new MemberManagement();
            memberManagement.ShowDialog();
        }

        private void btnEventManagement_Click(object sender, RoutedEventArgs e)
        {
            var eventManagement = new EventManagement();
            eventManagement.ShowDialog();
        }
    }
}
