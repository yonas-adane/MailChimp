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
            BatchService batchService = new BatchService(configuration);

            //Get list of batch status
            //to list all batch statuses, set GetStatus(true);
            Batch<BatchStatus> batchStatusList = batchService.GetStatus();

            //Get status for single batch
            BatchStatus batchStatus = batchService.GetStatus("111b63c08b");

        }
    }
}