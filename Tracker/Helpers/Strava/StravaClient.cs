using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;

namespace Tracker.Strava
{
    public class StravaClient
    {
        private readonly string clientId;
        private readonly string clientSecret;

        private string queryString;

        public StravaClient(string ClientId = "27093", string ClientSecret = "19eee5c3de5a167dafaba1d38fc3476965204c1a")
        {
            this.clientId = ClientId;
            this.clientSecret = ClientSecret;

            if (HttpContext.Current.Session["access_token"] != null)
                queryString = "Bearer " + HttpContext.Current.Session["access_token"];
        }

        public StravaAuthenticationResponse GetAccessToken(string code)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            RestClient client = new RestClient("https://www.strava.com/oauth/token");
            RestRequest request = new RestRequest(Method.POST);
            request.AddParameter("client_id", clientId);
            request.AddParameter("client_secret", clientSecret);
            request.AddParameter("code", code);

            IRestResponse<StravaAuthenticationResponse> response = client.Execute<StravaAuthenticationResponse>(request);
            var responseData = JsonConvert.DeserializeObject<StravaAuthenticationResponse>(response.Content);

            HttpContext.Current.Session["access_token"] = responseData.AccessToken;
            queryString = "Bearer " + responseData.AccessToken;

            return responseData;
        }

        public List<SummaryActivity> GetAthleteActivities()
        {
            RestClient client = new RestClient("https://www.strava.com/api/v3/activities");
            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", queryString);

            var response = client.Execute(request);
            var responseData = JsonConvert.DeserializeObject<List<SummaryActivity>>(response.Content);

            return responseData;
        }

        public DetailedActivity GetDetailedActivity(long id, bool includeAllEfforts)
        {
            RestClient client = new RestClient("https://www.strava.com/api/v3/activities/" + id);
            RestRequest request = new RestRequest(Method.GET);
            request.AddParameter("include_all_efforts", includeAllEfforts);
            request.AddHeader("Authorization", queryString);

            var response = client.Execute(request);
            var responseData = JsonConvert.DeserializeObject<DetailedActivity>(response.Content);

            return responseData;
        }
    }
}