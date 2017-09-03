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
        public List<T> batches = null;

        public int total_items = 0;

        public Batch()
        {
            batches = new List<T>();
        }
    }
    
}
