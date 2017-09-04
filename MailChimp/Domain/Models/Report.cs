using System;
using System.Collections.Generic;
using System.Text;

namespace MailChimp.Domain.Models
{
    /// <summary>
    /// Base class for reports
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class Report<T>
    {
        /// <summary>
        /// An array of objects, each representing a report resource
        /// </summary>
        public List<T> reports = null;

        /// <summary>
        /// An array of objects, each representing a campaign recipient.
        /// </summary>
        public List<T> sent_to = null;

        /// <summary>
        /// The total number of items matching the query regardless of pagination.
        /// </summary>
        public int total_items = 0;

        public Report()
        {
            reports = new List<T>();

            sent_to = new List<T>();

        }
    }
}
