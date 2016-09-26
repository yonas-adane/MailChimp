using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Core;
using System.Net;
using ICSharpCode.SharpZipLib.Zip;

namespace MailChimp_Batch
{
    class Program
    {

        public string credentials = string.Empty;
        public string endPointRootURL = string.Empty;
        public string apiKey = string.Empty;
        public string userName = string.Empty;
        public HttpClient client = null; //this can be WebClient if project is not based on .net core

        public Program()
        {
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

            endPointRootURL = ConfigurationManager.AppSettings.Get("endPointRootURL");
            apiKey = ConfigurationManager.AppSettings.Get("apiKey");
            userName = ConfigurationManager.AppSettings.Get("userName");

            client = new HttpClient();
            client.BaseAddress = new Uri(endPointRootURL);
            //credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("" + userName +":"+ apiKey ));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", apiKey);
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
                        //operation_id = l.id,
                        body = "",
                        @params = ""
                    });

                }

                //issue a batch operation request 
                //Task<string> batchResponse = p.requestBatchOperation(p.client, req);
                //var batchResult = batchResponse.Result;


                //check the status of batches
                Task<batchResponse> batchCheckResponse = p.checkBatchStatus(p.client);
                batchResponse batchCheckResult = batchCheckResponse.Result;

                //display result
                //print header
                //-ve spacing: left aligned
                string tableRowFormat = "| {0,-10} | {1,-10} | {2, 8} | {3,8} | {4,8} | {5,-25} | {6,-53} |";
                Console.WriteLine(tableRowFormat, "id", "status", "total", "finished", "errored", "completed", "response_body_url");
                foreach (var item in batchCheckResult.batches)
                {
                    Console.WriteLine(tableRowFormat,
                        item.id, item.status, item.total_operations, item.finished_operations, item.errored_operations, item.completed_at, String.IsNullOrEmpty(item.response_body_url) ? "" : item.response_body_url + "...");

                    Task<string> extracted = p.ExtractGZipSample(item.id,item.response_body_url);
                    string extractedVal = extracted.Result;

                    p.readMemberList(extractedVal);

                }



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }



        }


        public async Task<string> ExtractGZipSample(string batchId, string requestUri)
        {

            try
            {
                HttpClient webClient = new HttpClient();
                byte[] urlContents = await webClient.GetByteArrayAsync(requestUri);

                Stream stream = new MemoryStream(urlContents);
                GZipInputStream gzipStream = new GZipInputStream(stream);

                TarArchive tarArchive = TarArchive.CreateInputTarArchive(gzipStream);

                String tempFolderPath = Environment.GetEnvironmentVariable("TEMP");
                string extractFolderPath = tempFolderPath + @"\" + batchId + @"\";

                //when extracting, if path already exists, it will be overwritten.
                tarArchive.ExtractContents(extractFolderPath);
                tarArchive.Close();
                gzipStream.Close();
                stream.Close();

                return extractFolderPath;

            }
            catch 
            {
                throw;
            }

                
        }

        public void readMemberList(string extractFolderPath)
        {

            //loop over each file and read content of each json file.
            foreach (var file in Directory.EnumerateFiles(extractFolderPath))
            {

                memberResponse[] response = JsonConvert.DeserializeObject<memberResponse[]>(File.ReadAllText(file));

                memberRoot members = JsonConvert.DeserializeObject<memberRoot>(response[0].response);

                //use http://json2csharp.com/ to create classes from json script

                foreach(var member in members.members)
                {
                    Console.WriteLine("| {0,-10} | {1,-40} | {2,-20} |", member.list_id, member.email_address, member.timestamp_opt);
                }
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
                                operation_id = b.operation_id
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

        public void ExtractTGZ(String gzArchiveName, String destFolder)
        {

            Stream inStream = File.OpenRead(gzArchiveName);
            Stream gzipStream = new GZipInputStream(inStream);

            TarArchive tarArchive = TarArchive.CreateInputTarArchive(gzipStream);
            tarArchive.ExtractContents(destFolder);
            tarArchive.Close();

            gzipStream.Close();
            inStream.Close();
        }

        public class memberResponse
        {
            public string status_code { get; set; }
            public string operation_id { get; set; }
            public string response { get; set; }
        }


        //-------------------------------


        public class MergeFields
        {
            public string FNAME { get; set; }
            public string LNAME { get; set; }
            public string MMERGE3 { get; set; }
            public string MMERGE4 { get; set; }
        }

        public class Stats
        {
            public int avg_open_rate { get; set; }
            public int avg_click_rate { get; set; }
        }

        public class Location
        {
            public int latitude { get; set; }
            public int longitude { get; set; }
            public int gmtoff { get; set; }
            public int dstoff { get; set; }
            public string country_code { get; set; }
            public string timezone { get; set; }
        }

        public class Link
        {
            public string rel { get; set; }
            public string href { get; set; }
            public string method { get; set; }
            public string targetSchema { get; set; }
            public string schema { get; set; }
        }

        public class member
        {
            public string id { get; set; }
            public string email_address { get; set; }
            public string unique_email_id { get; set; }
            public string email_type { get; set; }
            public string status { get; set; }
            public MergeFields merge_fields { get; set; }
            public Stats stats { get; set; }
            public string ip_signup { get; set; }
            public string timestamp_signup { get; set; }
            public string ip_opt { get; set; }
            public string timestamp_opt { get; set; }
            public int member_rating { get; set; }
            public string last_changed { get; set; }
            public string language { get; set; }
            public bool vip { get; set; }
            public string email_client { get; set; }
            public Location location { get; set; }
            public string list_id { get; set; }
            public List<Link> _links { get; set; }
        }

        public class memberRoot
        {
            public List<member> members { get; set; }
        }

        //-------------------------------

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
            public int total_items { get; set; }
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
