using Microsoft.Framework.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MailChimp
{
    public class Program
    {
        public string credentials = string.Empty;
        public string endPointRootURL = string.Empty;
        public string apiKey = string.Empty;
        public string userName = string.Empty;
        public HttpClient client = null; //this can be WebClient if project is not based on .net core

        public Program()
        {

            var builder = new ConfigurationBuilder()
                     .AddJsonFile("config.json");
            /*
             * structure of the config.json file
             *
             * {
             *     "MailChimp": {
             *       "apiKey": "API_KEY",
             *       "userName": "USERNAME",
             *       "endPointRootURL": "ENDPOINT_URL"
             *     }
             * }
             *
             */

            var configuration = builder.Build();
            endPointRootURL = configuration["MailChimp:endPointRootURL"];
            apiKey = configuration["MailChimp:apiKey"];
            userName = configuration["MailChimp:userName"];

            client = new HttpClient();
            client.BaseAddress = new Uri(endPointRootURL);
            //credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("" + userName +":"+ apiKey ));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue ("Basic" , apiKey);
        }

        public static void Main(string[] args)
        {

            Program p = new Program();



            try
            {

                




                Task<listsResponse> lists = p.getLists(p.client);
                var listsResponse = lists.Result;
                List<batchRequest> req = new List<batchRequest>();

                /*
                 * Now loop over the list. 
                 * And issue a batch request to get members for each list.
                 */
                foreach (list l in listsResponse.lists)
                {

                    req.Add(new batchRequest
                    {
                        method = "GET",
                        path = "lists/" + l.id + "/members",
                        operation_id = l.id,
                        body = "",
                        @params = ""
                    });

                }

                //issue a batch operation request 
                //Task<string> batchResponse = p.requestBatchOperation(p.client, req);
                //var batchResult = batchResponse.Result;


                //check the status of batches
                Task<batchResponse> batchCheckResponse = p.checkBatchStatus(p.client);
                var batchCheckResult = batchCheckResponse.Result;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
           


        }

        /// <summary>
        /// Gets list of mailchimp lists collection 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public async Task<listsResponse> getLists(HttpClient client)
        {
            /*
             * Use the /lists?count=[NUMBER_OF_LISTS_TO_GET] to set a limit on number of lists returned. 
            * By expiriment, maximum number of count is 100.
            * To iterate over a large number of lists, use the offset parameter.
            * For really large number of lists, you may need to use a batch request. 
            */
            var response = await client.GetAsync(endPointRootURL + "/lists");
            var contents = await response.Content.ReadAsStringAsync();

            /*
             * Structure of lists
             * listsResponse (a single response object that contains an overall info of the list collection. like number of lists)
             * -list (this is the actual list of lists collection). 
             */
            listsResponse lists = JsonConvert.DeserializeObject<listsResponse>(contents);  //convert the json in the response object into a list.

            return lists;
        }

        public async Task<string> requestBatchOperation(HttpClient client, List<batchRequest> request)
        {
            //build batch operation request json string
            //for full details on other options to include in the string
            //refer to mailchimp Batch Operations guide
            JObject payload = JObject.FromObject(
                new
                {
                    operations =
                            from b in request
                            select new
                            {
                                method = b.method,
                                path = b.path,
                                operation_id =  b.operation_id 
                            }
                });

            var content = new StringContent(payload.ToString(), Encoding.UTF8, "application/json");

            var result = await client.PostAsync(client.BaseAddress + "/batches", content);

            return result.ToString();

        }

        public async Task<batchResponse> checkBatchStatus(HttpClient client)
        {

            var response = await client.GetAsync(client.BaseAddress + "/batches?count=100");
            var contents = await response.Content.ReadAsStringAsync();

            batchResponse batch = JsonConvert.DeserializeObject<batchResponse>(contents);  //convert the json in the response object into a list.

            return batch;
        }

        public class listsResponse
        {
            public List<list> lists { get; set; }
            public int total_items { get; set; }
        }

        public class list
        {
            public string id { get; set; }
            public string name { get; set; }
        }
        
        public class batchRequest
        {
            //method and path are required
           public string method { get; set; }
           public string path { get; set; } //The relative path of the operation
           
           public string operation_id { get; set; } //A string you provide that identifies the operation
           public string @params { get; set; } //Any URL params, only used for GET 
           public string body { get; set; } //The JSON payload for PUT, POST, or PATCH

        }

        public class batchResponse
        {
            public List<batch> batches { get; set; }
            public int total_items { get;  set;}
        }

        //for more detail on the fields below,
        //go to: http://developer.mailchimp.com/documentation/mailchimp/reference/batches/
        public class batch
        {
            public string id { get; set; }
            public string status { get; set; }
            public int total_operations { get; set; }
            public int finished_operations { get; set; }
            public int errored_operations { get; set; }
            public string submitted_at { get; set; }
            public string completed_at { get; set; }
            public string response_body_url { get; set; }
        }


    }
}
