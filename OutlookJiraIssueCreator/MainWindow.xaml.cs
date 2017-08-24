using Microsoft.Office.Interop.Outlook;
using OutlookJiraIssueCreator.Classes;
using OutlookJiraIssueCreator.Forms;
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

        string[] labels;
        string user = "tratlk";
        string password = "t#aru1440118";
        MailItem mail;
        int labelIndex = 0;

        public MainWindow()
        {
            InitializeComponent();
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

        }

        
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            this.prbLoading.Visibility = Visibility.Visible;
            this.btnCreate.Content = "Creating Issue...";

            this.IsEnabled = false;

            this.project = this.txtProject.Text;
            this.type = this.cmbIssueType.Text;
            this.priority = this.cmbPriority.Text;
            this.summary = this.txtSummary.Text;
            this.description = this.txtDesc.Text;
            this.labels = this.txtLabels.Text.Split(' ');

            this.worker.RunWorkerAsync();

        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.btnCreate.Content = "Create Issue";
            this.prbLoading.Visibility = Visibility.Hidden;
            this.IsEnabled = true;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.CreateIssue();
        }

        bool CreateIssue()
        {
            RestClient restClient = new RestClient(server);
            RestRequest request = new RestRequest("/rest/auth/1/session", Method.GET);
            bool bReturn = false;
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.JiraUsername) ||
                string.IsNullOrWhiteSpace(Properties.Settings.Default.JiraPassword))
            {
                frmJiraLogin jiraLogin = new frmJiraLogin();
                jiraLogin.ShowDialog();
            }
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
                            key = this.project
                        },
                        summary = this.summary,
                        description = this.description,
                        issuetype = new
                        {
                            name = this.type
                        },
                        priority = new
                        {
                            name = this.priority
                        },
                        labels = this.labels
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
            }
            else
            {
                try
                {
                    Authenticator.AuthenticateUser(Properties.Settings.Default.JiraServer, Properties.Settings.Default.JiraUsername, Properties.Settings.Default.JiraPassword);

                }
                catch (System.Exception)
                {
                    throw;
                }
            }
            bReturn = false;
            
            return bReturn;
        }

        private void btnSettings_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            frmJiraLogin jiraLogin = new frmJiraLogin();
            jiraLogin.ShowDialog();
        }
    }
}
