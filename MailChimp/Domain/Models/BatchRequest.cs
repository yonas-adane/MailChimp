using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MailChimp.Domain.Models
{
    /// <summary>
    /// Creating a batch request
    /// </summary>
    public class BatchRequest 
    {
        /// <summary>
        /// An array of objects that describe operations to perform
        /// </summary>
        public List<BatchOperation> Operations { get; set; }

    }

   
 
}
