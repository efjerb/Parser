using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Policy;

namespace RevitToRDFConverter
{
    public class RepositoryResponse
    {
        public Dictionary<string, List<string>> head;
        public Results results;
    }
    
    public class Results
    {
        public List<Dictionary<string, Dictionary<string, string>>> bindings;
    }

    public class HttpClientHelper
    {
        private static HttpClient client = new HttpClient();

        public static async Task<string> POSTDataAsync(string data)
        {

            var data1 = data.ToString();
            var data2 = new StringContent(JsonConvert.SerializeObject(data1), Encoding.UTF8, "text/turtle");

            var url = "http://localhost:3500/Bot";
            HttpResponseMessage response = await client.PostAsync(url, data2);
            string result = response.Content.ReadAsStringAsync().Result;
            return result;
        }
    }

    public class GraphDBHTTPHelper
    {
        private static HttpClient client = new HttpClient();
        public static bool OverwriteRepository(string data)
        {
            string baseUrl = PromptGraphDBURL();

            (string repositoryID, string graphName) = PromptGraphDBRepositoryID(baseUrl);

            var data1 = new StringContent(data, Encoding.UTF8, "text/turtle");

            var url = $"http://{baseUrl}/repositories/{repositoryID}/statements";
            HttpResponseMessage response = client.PutAsync(url, data1).Result;
            
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return true;
            string returnValue = response.Content.ReadAsStringAsync().Result;
            throw new Exception($"Failed to POST data: ({response.StatusCode}): {returnValue}");

        }
        public static bool OverwriteGraph(string data)
        {
            string baseUrl = PromptGraphDBURL();

            (string repositoryID, string graphName) = PromptGraphDBRepositoryID(baseUrl);

            var url = $"http://{baseUrl}/repositories/{repositoryID}/rdf-graphs/{graphName}";

            // Delete the graph
            HttpResponseMessage deleteRespone = client.DeleteAsync(url).Result;


            var data1 = new StringContent(data, Encoding.UTF8, "text/turtle");

            HttpResponseMessage postResponse = client.PostAsync(url, data1).Result;

            if (postResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
                return true;
            string returnValue = postResponse.Content.ReadAsStringAsync().Result;
            throw new Exception($"Failed to POST data: ({postResponse.StatusCode}): {returnValue}");

        }
        public static string PromptGraphDBURL()
        {
            GraphDBURLForm prompt = new GraphDBURLForm();
            prompt.ShowDialog();
            string url = prompt.url;
            return url;
        }
        public static (string reposirotyID, string graphName) PromptGraphDBRepositoryID(string baseUrl)
        {
            List<string> repositoryList = GetReposirotyIDs(baseUrl);
            GraphDBForm prompt = new GraphDBForm(repositoryList);
            prompt.ShowDialog();
            string repositoryID = prompt.repositoryId;
            string graphName = prompt.graphName;

            return (repositoryID, graphName);
        }

        private static List<string> GetReposirotyIDs(string baseUrl)
        {
            string url = $"http://{baseUrl}/repositories";
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/sparql-results+json"));
            
            HttpResponseMessage response = client.GetAsync(url).Result;
            string returnValue = response.Content.ReadAsStringAsync().Result;
            //RepositoryResponse json = System.Text.Json.JsonSerializer.Deserialize<RepositoryResponse>(returnValue);
            dynamic json = JsonConvert.DeserializeObject(returnValue);
            List<string> repositoryIDs = new List<string>();

            foreach (var item in json.results.bindings)
            {
                repositoryIDs.Add(item.id.value.ToString());
            }
            return repositoryIDs;
        }
    }


     
}

