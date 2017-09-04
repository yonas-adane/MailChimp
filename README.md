# MailChimp API 3.0 Batch Operation C# demo
The Batch Operation API is the coolest solution to work with syncing large data such as campaign reports and member list with applications like Blackbaud CRM and SalesForce. 

With Batch Operation, all that is needed is to put a request in a batch, wait for the batch to complete, and grab/process the data from the completed batch. 

Recently Batch Operation API supports WebHooks to notify a client a batch is completed. This is super useful avoiding the need to periodically manually (programmatically) check if a batch is completed.

Below is a sample code to show the usage of this project. I'll add support for more items as I continue to expolore the API.
```C#
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
```