using System;
using System.Collections.Generic;
using System.Text;

namespace MailChimp.Domain.Models
{
    public class BatchOperation
    {
        /// <summary>
        /// The HTTP method to be used for the operation
        /// ["GET", "POST", "PUT", "PATCH"]
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The relative path to be used for the operation
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// A string containing the JSON body to be used with the request
        /// </summary>
        public string Body { get; set; }
    }
}
