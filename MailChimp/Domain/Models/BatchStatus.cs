using System;
using System.Collections.Generic;
using System.Text;

namespace MailChimp.Domain.Models
{
    /// <summary>
    /// The status of a batch request
    /// </summary>
    public class BatchStatus
    {
        /// <summary>
        /// A string that uniquely identifieds this batch request
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The status of the batch call
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The total number of operations to be completed as part of this batch request. 
        /// For GET requests requiring pagination, each page will count as a seperate operation
        /// </summary>
        public int TotalOperations { get; set; }

        /// <summary>
        /// The number of operations that have finished. This number includes operations that returned an error.
        /// </summary>
        public int FinishedOperations { get; set; }

        /// <summary>
        /// The number of finished operations that returned an error
        /// </summary>
        public int ErroredOperations { get; set; }

        /// <summary>
        /// The time and date at which the batch request was recieved by the server
        /// </summary>
        public string SubmittedAt { get; set; }

        /// <summary>
        /// The time and date at which all operations in the batch request had been completed
        /// </summary>
        public string CompletedAt { get; set; }

        /// <summary>
        /// The url of the gzipped archive of the results of all the operations
        /// </summary>
        public string ResponseBodyUrl { get; set; }
    }
}
