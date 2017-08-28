using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OutlookJiraIssueCreator.Models
{
    class Jira : INotifyPropertyChanged
    {
        private string _jiraServer;
        private string _jiraUsername;
        private string _jiraPassword;

        private string _jiraProject;
        private string _jiraIssueType;
        private string _jiraIssueSummary;
        private string _jiraIssueDescription;
        private string _jiraIssuePriority;
        private string _jiraIssueCategory;
        private string _jiraIssueLabels;

        public string JiraServer { get => _jiraServer; set => this.MutateVerbose(ref _jiraServer, value, RaisePropertyChanged()); }
        public string JiraUsername { get => _jiraUsername; set => this.MutateVerbose(ref _jiraUsername, value, RaisePropertyChanged()); }
        public string JiraPassword { get => _jiraPassword; set => this.MutateVerbose(ref _jiraPassword, value, RaisePropertyChanged()); }
        public string JiraProject { get => _jiraProject; set => this.MutateVerbose(ref _jiraProject, value, RaisePropertyChanged()); }
        public string JiraIssueType { get => _jiraIssueType; set => this.MutateVerbose(ref _jiraIssueType, value, RaisePropertyChanged()); }
        public string JiraIssueSummary { get => _jiraIssueSummary; set => this.MutateVerbose(ref _jiraIssueSummary, value, RaisePropertyChanged()); }
        public string JiraIssueDescription { get => _jiraIssueDescription; set => this.MutateVerbose(ref _jiraIssueDescription, value, RaisePropertyChanged()); }
        public string JiraIssuePriority { get => _jiraIssuePriority; set => this.MutateVerbose(ref _jiraIssuePriority, value, RaisePropertyChanged()); }
        public string JiraIssueCategory { get => _jiraIssueCategory; set => this.MutateVerbose(ref _jiraIssueCategory, value, RaisePropertyChanged()); }
        public string JiraIssueLabels { get => _jiraIssueLabels; set => this.MutateVerbose(ref _jiraIssueLabels, value, RaisePropertyChanged()); }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }

    public static class NotifyPropertyChangedExtension
    {
        public static void MutateVerbose<TField>(this INotifyPropertyChanged instance, ref TField field, TField newValue, Action<PropertyChangedEventArgs> raise, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TField>.Default.Equals(field, newValue)) return;
            field = newValue;
            raise?.Invoke(new PropertyChangedEventArgs(propertyName));
        }
    }
}
