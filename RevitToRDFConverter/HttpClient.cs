using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
        public static bool PutData(string data)
        {
            string repositoryID = PromptGraphDBRepositoryID();

            var data1 = new StringContent(data, Encoding.UTF8, "text/turtle");

            var url = $"http://localhost:7200/repositories/{repositoryID}/statements";
            HttpResponseMessage response = client.PutAsync(url, data1).Result;
            
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return true;
            string returnValue = response.Content.ReadAsStringAsync().Result;
            throw new Exception($"Failed to POST data: ({response.StatusCode}): {returnValue}");

        }
        public static string PromptGraphDBRepositoryID()
        {
            List<string> repositoryList = GetReposirotyIDs();
            GraphDBForm prompt = new GraphDBForm(repositoryList);
            prompt.ShowDialog();
            string repositoryID = prompt.repositoryId;

            return repositoryID;
        }
        private static List<string> GetReposirotyIDs()
        {
            var url = $"http://localhost:7200/repositories";
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

