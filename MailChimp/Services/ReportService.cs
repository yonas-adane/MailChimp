using MailChimp.Domain.Interfaces;
using MailChimp.Domain.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace MailChimp.Services
{
    class ReportService<T> : IReportService<T>
    {

        private HttpClient httpClient = null;

        private AppSettings appSettings = null;

        private static Uri reportApiUri = null;

        //the default max number of rows returned by the batch api
        private static int DEFAULT_MAX_COUNT_PER_PAGE = 100;

        public ReportService(ConfigurationBase configuration)
        {
            this.httpClient = configuration.httpClient;

            this.appSettings = configuration.appSettings;

            reportApiUri = new Uri(string.Concat(httpClient.BaseAddress, appSettings.Reports));

        }

        /// <summary>
        /// Get campaign reports
        /// </summary>
        /// <param name="includeAll"></param>
        /// <returns></returns>
        public Report<T> GetReport(bool includeAll = false)
        {

            int offset = 0;

            var result = httpClient.GetAsync(string.Concat(reportApiUri, string.Concat("?offset=", offset, "&count=", DEFAULT_MAX_COUNT_PER_PAGE))).Result.Content.ReadAsStringAsync().Result;

            var reportList = JObject.Parse(result).ToObject<Report<T>>();

            return reportList;
        }

        /// <summary>
        /// Get a specific campaign report
        /// </summary>
        /// <param name="campaignId"></param>
        /// <returns></returns>
        public T GetReport(string campaignId)
        {
            var result = httpClient.GetAsync(string.Concat(reportApiUri, "/", campaignId)).Result.Content.ReadAsStringAsync().Result;

            T report = JObject.Parse(result).ToObject<T>();

            return report;
        }

        public Report<T> GetSentTo(string campaignId)
        {
            int offset = 0;

            var result = httpClient.GetAsync(string.Concat(reportApiUri, "/", campaignId, "/sent-to", string.Concat("?offset=", offset, "&count=", DEFAULT_MAX_COUNT_PER_PAGE))).Result.Content.ReadAsStringAsync().Result;

            var sentToList = JObject.Parse(result).ToObject<Report<T>>();

            return sentToList;

        }
        
    }
}
