using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using System.Net;
using MailChimp_Batch.Models;

namespace MailChimp_Batch
{
    class Program
    {

        public string credentials = string.Empty;
        public string endPointRootUrl = string.Empty;
        public string apiKey = string.Empty;
        public string userName = string.Empty;
        public WebClient client = null;

        //default constructor for initalizing variables.
        public Program()
        {
            //info for connecting to the api
            endPointRootUrl = ConfigurationManager.AppSettings.Get("endPointRootURL");
            apiKey = ConfigurationManager.AppSettings.Get("apiKey");
            userName = ConfigurationManager.AppSettings.Get("userName");

            //send credentials to get authorized to access the api.
            client = new WebClient();
            credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("" + userName +":"+ apiKey ));
            client.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);

        }

        public static void Main(string[] args)
        {

            try
            {

                Program p = new Program();

                //get list of lists
                List<List> lists = p.getList().lists;

                //for lists already in the exclude file, don't issue a batch operation request
                //NOTE: the file "Exclude.json" is located in the bin\debug folder.
                string fileContent = File.ReadAllText(@"Exclude.json");

                List<Batch> excludeList = new List<Batch> ();
                List<List> requestBatch = new List<List> ();

                if (fileContent.Length > 0)
                {
                    excludeList = JArray.Parse(fileContent).ToObject<List<Batch>>();

                     requestBatch = (from l in lists
                                       join e in excludeList
                                       on l.id equals e.listId
                                       into le //left join list with the exclusion list
                                       from b in le.DefaultIfEmpty()
                                       where (b == null) //only pull lists with no batch id
                                       select l).ToList<List>();
                }
                else
                {
                    requestBatch = (from l in lists
                                    select l).ToList<List>();
                }

                //issue a batch request operation 
                foreach (var l in requestBatch)
                {
                    Batch batch = p.postBatch("/lists/" + l.id + "/members");
                    batch.listId = l.id;

                    //once batch operation request is posted, add list to the exclusion list.
                    excludeList.Add(batch);
                }

                //Update exclusion list file.
                File.WriteAllText(@"Exclude.json", JsonConvert.SerializeObject(excludeList));

                //check the status of batch operations
                //if completed (status == completed and errored_operations = 0) 
                //download file, untarr and unzip file. 
                //then update the CRM.

                //reload exclusion list
                excludeList = JArray.Parse(File.ReadAllText(@"Exclude.json")).ToObject<List<Batch>>();

                //ready list contains list of batches ready to be downloade/extracted
                //Note: once the data is downloaded and extracted, it'll be removed from the exclusion list
                List<Batch> readyList = new List<Batch>(excludeList);

                //loop through the exclusion list and see if we've batches ready for download.
                foreach (var e in readyList)
                {
                    string downloadUrl = p.getDownloadUrl(e.id);

                    if (downloadUrl.Length > 0)
                    {
                        //download file
                        string extractFolder = p.extractFile(e.id, downloadUrl);

                        //display downloaded list (Optional)
                        p.getMemberList(extractFolder);

                        //update CRM list.
                        //Call the CRM's API to proceed working on the downloaded list.

                        //once the CRM task is done, update the exclude list so 
                        //that we can download the list again in the future 
                        //(member lists constantly change).
                        excludeList.Remove(e);

                        //write the updated list back to the file.
                        File.WriteAllText(@"Exclude.json", JsonConvert.SerializeObject(excludeList));

                    }


                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("All done!");
            Console.Read();
        }


        /// <summary>
        /// Get information about all lists
        /// </summary>
        /// <returns></returns>
        public ListResponse getList()
        {
            /*
            * Use the /lists?count=[NUMBER_OF_LISTS_TO_GET] to set a limit on number of lists returned. 
           * By expiriment, maximum number of count is 100.
           * To iterate over a large number of lists, use the offset parameter.
           * For really large number of lists, you may need to use a batch request. 
           */
            var response = client.DownloadString(endPointRootUrl + "/lists");

            ListResponse lists = JsonConvert.DeserializeObject<ListResponse>(response);  //convert the json in the response object into a list.

            return lists;
        }

        /// <summary>
        /// Begin processing a batch operations request.
        /// </summary>
        public Batch postBatch(string path)
        {

            //build payload string
            string payload = String.Format(@"{{""operations"": [{{""method"": ""GET"", ""path"": ""{0}""}} ]}}",path);

            var response = client.UploadString(endPointRootUrl + "/batches", payload);

            Batch batch = JsonConvert.DeserializeObject<Batch>(response);

            return batch;

        }

        /// <summary>
        /// Checkes if a batch operation is completed
        /// </summary>
        /// <returns>The Url to download the data file</returns>
        public string getDownloadUrl(string batchId)
        {
            var response = client.DownloadString(endPointRootUrl + "/batches/" + batchId);

            Batch batch = JsonConvert.DeserializeObject<Batch>(response);

            if (batch.errored_operations == 0 && batch.status == "finished")
                return batch.response_body_url;
            else
                return string.Empty;

        }

        
        /// <summary>
        /// Downloads the datafile and extracts its contents
        /// </summary>
        /// <param name="batchId">The id of the batch. Used for naming the folder where the files will be extracted</param>
        /// <param name="requestUri">The url to download the data file</param>
        /// <returns>The path to the folder where the extracted data files are located</returns>
        public string extractFile(string batchId, string requestUri)
        {

            try
            {
                WebClient downloadClient = new WebClient();
                byte[] urlContents = downloadClient.DownloadData(requestUri);

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


        /// <summary>
        /// Gets member list
        /// </summary>
        /// <param name="extractFolderPath">The path to the folder where data files are extracted.</param>
        public void getMemberList(string extractFolderPath)
        {

            //loop over each file and read content of each json file.
            foreach (var file in Directory.EnumerateFiles(extractFolderPath))
            {

                MemberResponse[] response = JsonConvert.DeserializeObject<MemberResponse[]>(File.ReadAllText(file));

                Members members = JsonConvert.DeserializeObject<Members>(response[0].response);

                foreach(var member in members.members)
                {
                    Console.WriteLine("| {0,-10} | {1,-40} | {2,-20} |", member.list_id, member.email_address, member.timestamp_opt);
                }
            }

        }
             


    }
}
