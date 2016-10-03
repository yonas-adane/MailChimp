using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailChimp_Batch.Models
{
    /// <summary>
    /// Information about a Batch
    /// </summary>
    public class Batch
    {
        public string id { get; set; }
        public string listId { get; set; }
        public string status { get; set; }
        public int total_operations { get; set; }
        public int finished_operations { get; set; }
        public int errored_operations { get; set; }
        public string submitted_at { get; set; }
        public string completed_at { get; set; }
        public string response_body_url { get; set; }
    }

    /// <summary>
    /// Information about Batches
    /// </summary>
    public class BatchResponse
    {
        public List<Batch> batches { get; set; }
        public int total_items { get; set; }
    }

}

