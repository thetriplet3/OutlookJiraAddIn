using OutlookJiraIssueCreator.Classes;
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

namespace OutlookJiraIssueCreator.Forms
{
    /// <summary>
    /// Interaction logic for frmJiraLogin.xaml
    /// </summary>
    public partial class frmJiraLogin : Window
    {
        public frmJiraLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Authenticator.AuthenticateUser(this.txtServer.Text, this.txtUsername.Text, this.txtPassword.Password);
                Properties.Settings.Default.JiraServer = txtServer.Text;
                Properties.Settings.Default.JiraUsername = txtUsername.Text;
                Properties.Settings.Default.JiraPassword = txtPassword.Password;
                Properties.Settings.Default.Save();
                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error logging to JIRA.");
            }
        }
    }
}
