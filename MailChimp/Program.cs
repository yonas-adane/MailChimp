using MailChimp.Domain.Models;
using MailChimp.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MailChimp
{
    class Program
    {

        static void Main(string[] args)
        {

            //Get instance of WebClient and appSettings
            ConfigurationBase configuration = ConfigurationBase.Instance;

            //Start batch services
            BatchService<BatchStatus> batchService = new BatchService<BatchStatus>(configuration);

            //Get list of batch status
            //to list all batch statuses, set GetStatus(true);
            var batchStatusList = batchService.GetStatus();

            //Get status for single batch
            var batchStatus = batchService.GetStatus("111b63c08b");

            //Start report services
            ReportService<Campaign> reportService = new ReportService<Campaign>(configuration);

            //Get list of reports
            var reportList = reportService.GetReport();

            //Get report of signle campaign
            var report = reportService.GetReport("eaaf0b3b623");

            //Get Sent-To report for a campaign
            var sentToReport = reportService.GetSentTo("eaaf0b3b623");

            //Get the full list of campaign receipients using batch operation
            //Issue a batch request
            //Check status
            //Download and process file


        }
    }
}