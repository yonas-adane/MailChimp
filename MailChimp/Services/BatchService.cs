using MailChimp.Domain.Interfaces;
using MailChimp.Domain.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace MailChimp.Services
{
    public class BatchService : IBatchService
    {
        private HttpClient httpClient = null;

        private AppSettings appSettings = null;

        private static Uri batchApiUri = null;

        //the default max number of rows returned by the batch api
        private static int DEFAULT_MAX_COUNT_PER_PAGE = 100;

        public BatchService(ConfigurationBase configuration)
        {
            this.httpClient = configuration.httpClient;

            this.appSettings = configuration.appSettings;

            batchApiUri = new Uri(string.Concat(httpClient.BaseAddress, appSettings.Batches));

        }

        /// <summary>
        /// Get a list of batch requests    
        /// </summary>
        /// <param name="includeAll">Optional: default false - maximum number of rows is limited by the DEFAULT_MAX_COUNT_PER_PAGE property. 
        /// if set to true all rows will be included in the list - slower if list is large</param>
        /// <returns></returns>
        public Batch<BatchStatus> GetStatus(bool includeAll = false)
        {
            int offset = 0;

            var result = httpClient.GetAsync(string.Concat(batchApiUri, string.Concat("?offset=", offset, "&count=", DEFAULT_MAX_COUNT_PER_PAGE))).Result.Content.ReadAsStringAsync().Result;

            Batch<BatchStatus> batchStatusList = JObject.Parse(result).ToObject<Batch<BatchStatus>>();

            //if user requested all rows proceed pulling all rows.
            if (includeAll)
            {

                //for lists more than DEFAULT_MAX_COUNT_PER_PAGE, we need to paginate to get all batches included in the result list
                if (batchStatusList.total_items > DEFAULT_MAX_COUNT_PER_PAGE)
                {
                    int totalPages = (int)Math.Ceiling(batchStatusList.total_items / (Decimal)DEFAULT_MAX_COUNT_PER_PAGE);

                    for (int i = 1; i < totalPages; i++)
                    {
                        offset += DEFAULT_MAX_COUNT_PER_PAGE;

                        var pagedResult = httpClient.GetAsync(string.Concat(batchApiUri, string.Concat("?offset=", offset, "&count=", DEFAULT_MAX_COUNT_PER_PAGE))).Result.Content.ReadAsStringAsync().Result;

                        //add it to the existing list
                        batchStatusList.batches.AddRange(JObject.Parse(pagedResult).ToObject<Batch<BatchStatus>>().batches);

                    }

                }

            }

            return batchStatusList;
        }

        /// <summary>
        /// Get the status of a batch operation request
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public BatchStatus GetStatus(string batchId)
        {

            var result = httpClient.GetAsync(string.Concat(batchApiUri, "/" , batchId)).Result.Content.ReadAsStringAsync().Result;

            BatchStatus batchStatus = JObject.Parse(result).ToObject<BatchStatus>();

            return batchStatus;
        }


    }
}
