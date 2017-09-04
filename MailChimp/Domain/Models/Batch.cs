using System;
using System.Collections.Generic;
using System.Text;

namespace MailChimp.Domain.Models
{

    /// <summary>
    /// Batch root 
    /// </summary>
    public class Batch<T>
    {
        /// <summary>
        /// An array of objects, each representing a batch resource
        /// </summary>
        public List<T> batches = null;

        /// <summary>
        /// The total number of items matching the query regardless of pagination.
        /// </summary>
        public int total_items = 0;

        public Batch()
        {
            batches = new List<T>();
        }
    }
    
}
