using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OutlookJiraIssueCreator.Classes
{
    class Authenticator
    {
        internal static bool AuthenticateUser(string server, string username, string password)
        {
            RestClient restClient = new RestClient(server);
            RestRequest request = new RestRequest("/rest/auth/1/session", Method.POST);

            request.AddHeader("Authentication", "Basic");
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { username = username, password = password });

            HttpStatusCode status = restClient.Execute(request).StatusCode;

            if (status == HttpStatusCode.OK)
            {
                return true;
            }
            else if (status == HttpStatusCode.Unauthorized)
            {
                throw new Exception(Properties.Resources.ErrorAuthUnauthorized);
            }
            else if (status == HttpStatusCode.Forbidden)
            {
                throw new Exception(Properties.Resources.ErrorAuthForbidden);
            }
            else
            {
                throw new Exception(Properties.Resources.ErrorAuthOther);
            }
        }
    }
}
