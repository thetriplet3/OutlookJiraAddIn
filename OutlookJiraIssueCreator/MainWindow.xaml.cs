using MaterialDesignThemes.Wpf;
using Microsoft.Office.Interop.Outlook;
using OutlookJiraIssueCreator.Classes;
using OutlookJiraIssueCreator.Forms;
using OutlookJiraIssueCreator.Models;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace OutlookJiraIssueCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        BackgroundWorker worker;

        string project = "PJS";
        string server = "http://jiratest";
        string type = "Bug";
        string priority = "Three";
        string summary;
        string description;
        bool isAuthed = false;

        string[] labels;
        string user = "tratlk";
        string password = "t#aru1440118";
        MailItem mail;
        int labelIndex = 0;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new Jira();

            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_workCompleted;

            txtProject.Text = Properties.Settings.Default.JiraProject;
            txtCategory.Text = Properties.Settings.Default.JiraCategory;

            cmbJiraServer.Text = Properties.Settings.Default.JiraServer;
            txtUsername.Text = Properties.Settings.Default.JiraUsername;
            txtPassword.Password = Properties.Settings.Default.JiraPassword;

        }

        
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.JiraServer) ||
                string.IsNullOrWhiteSpace(Properties.Settings.Default.JiraUsername) ||
                string.IsNullOrWhiteSpace(Properties.Settings.Default.JiraPassword))
            {
                ppbSettings.IsPopupOpen = true;
            }
            else
            {
                isAuthed = true;
            }

            if (isAuthed)
            {
                prbLoading.Visibility = Visibility.Visible;
                btnCreate.Content = "Creating Issue...";

                IsEnabled = false;

                project = txtProject.Text;
                type = cmbIssueType.Text;
                priority = cmbPriority.Text;
                summary = txtSummary.Text;
                description = txtDesc.Text;
                labels = txtLabels.Text.Split(' ');

                worker.RunWorkerAsync(); 
            }

        }

        private void worker_workCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                Properties.Settings.Default.JiraCategory = txtCategory.Text;
                Properties.Settings.Default.JiraProject = txtProject.Text;

                Properties.Settings.Default.Save();
            }
            btnCreate.Content = "Create Issue";
            prbLoading.Visibility = Visibility.Hidden;
            IsEnabled = true;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            CreateIssue();
        }

        bool CreateIssue()
        {
            
            RestClient restClient = new RestClient(server);
            RestRequest request = new RestRequest("/rest/auth/1/session", Method.GET);
            bool bReturn = false;

            restClient.Authenticator = new HttpBasicAuthenticator(Properties.Settings.Default.JiraUsername, Properties.Settings.Default.JiraPassword);
            request.RequestFormat = RestSharp.DataFormat.Json;

            IRestResponse response = restClient.Execute(request);
            HttpStatusCode status = response.StatusCode;

            if (status == HttpStatusCode.OK)
            {
                var obj = new
                {
                    fields = new
                    {
                        project = new
                        {
                            key = project
                        },
                        summary = summary,
                        description = description,
                        issuetype = new
                        {
                            name = type
                        },
                        priority = new
                        {
                            name = priority
                        },
                        labels = labels
                    }
                };


                request.Resource = "/rest/api/2/issue/";
                request.Method = Method.POST;
                request.AddBody(obj);
                response = restClient.Execute(request);

                status = response.StatusCode;
                if (status == HttpStatusCode.Created)
                {/*
                    DirectoryInfo di = Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\OutlookJiraAddin\");
                    String savepath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\OutlookJiraAddin\" + "message" + ".msg";
                    mail.SaveAs(savepath);
                    */
                    JsonObject jiraResponse = (JsonObject)SimpleJson.DeserializeObject(response.Content);
                    /*
                    request = new RestRequest();
                    request.Resource = String.Format("/rest/api/2/issue/{0}/attachments", jiraResponse["key"]);
                    request.Method = Method.POST;
                    request.AddHeader("X-Atlassian-Token", "nocheck");
                    request.AddHeader("Content-Type", "multipart/form-data");
                    request.AddFileBytes("file", File.ReadAllBytes(savepath), String.Format("{0}.msg", mail.Subject), "application/octet-stream");

                    response = restClient.Execute(request);
                    */
                    String url = "http://jiratest/browse/" + jiraResponse["key"];
                    System.Diagnostics.Process.Start(url);
                    bReturn = true;
                }
                else
                {
                    throw new System.Exception(status.ToString());
                }
            }
            else
            {
                try
                {
                    Authenticator.AuthenticateUser(Properties.Settings.Default.JiraServer, Properties.Settings.Default.JiraUsername, Properties.Settings.Default.JiraPassword);

                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }
            bReturn = false;
            
            return bReturn;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                isAuthed = Authenticator.AuthenticateUser(cmbJiraServer.Text, txtUsername.Text, txtPassword.Password);
                Properties.Settings.Default.JiraServer = cmbJiraServer.Text;
                Properties.Settings.Default.JiraUsername = txtUsername.Text;
                Properties.Settings.Default.JiraPassword = txtPassword.Password;
                Properties.Settings.Default.Save();
                txtHead.Text = "Logged in succefully!";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
