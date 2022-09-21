using SiteIndexer.Services.Solr.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace SiteIndexer.Services.Solr
{
    public interface ISolrApiService
    {
        //IssueResponseApiModel GetIssues(string jiraUrl, string projectCode, int resultCount, int startAt, List<string> issueTypes);
    }

    public class SolrApiService : ISolrApiService
    {
        #region Constructor

        protected readonly ISolrClient Client;

        public SolrApiService(ISolrClient client)
        {
            Client = client;
        }

        #endregion

        #region Context

        //public object AddToIndex()
        //{
        //    var client = new RestClient("http://localhost:900/solr/webcrawl/update?commitWithin=1000");
        //    client.Timeout = -1;
        //    var request = new RestRequest(Method.POST);
        //    request.AddHeader("Content-Type", "application/json");
        //    request.AddParameter("application/json", "[{\r\n    \"id\":\"12345678\",\r\n    \"name\":[\"mark mark mark\"],\r\n    \"features\":[\"No accents here\", \"This document is very shiny (translated)\"],\r\n}]", ParameterType.RequestBody);
        //    IRestResponse response = client.Execute(request);
        //    Console.WriteLine(response.Content);
        //}

        //public IssueResponseApiModel GetIssues(string jiraUrl, string projectCode, int resultCount, int startAt, List<string> issueTypes)
        //{
        //    var jqlPath = $"project='{projectCode}'";
            
        //    //this fails if you query for issue types that aren't in the specific endpoint so the issue types must match the endpoint
        //    //if(issueTypes.Count > 0)
        //    //{
        //    //    var issueConditionList = new List<string>();
        //    //    foreach (var issueType in issueTypes)
        //    //    {
        //    //        issueConditionList.Add($"issuetype='{issueType}'");
        //    //    }

        //    //    jqlPath = $"{jqlPath} AND ({string.Join(" OR ", issueConditionList)})";
        //    //}

        //    var apiUrl = $"{jiraUrl}/rest/api/3/search?jql={HttpUtility.UrlEncode(jqlPath)}&maxResults={resultCount}&startAt={startAt}";
        //    var response = Client.SendGet<IssueResponseApiModel>(apiUrl);
            
        //    return response;
        //}

        #endregion
    }
}