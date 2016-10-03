using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailChimp_Batch.Models
{

    /// <summary>
    /// Information about a list
    /// </summary>
    public class List
    {
        public string id { get; set; }
        public string name { get; set; }
        public string permission_reminder { get; set; }
        public bool use_archive_bar { get; set; }
        public string notify_on_subscribe { get; set; }
        public string notify_on_unsubscribe { get; set; }
        public string date_created { get; set; }
        public int list_rating { get; set; }
        public bool email_type_option { get; set; }
        public string subscribe_url_short { get; set; }
        public string subscribe_url_long { get; set; }
        public string beamer_address { get; set; }
        public string visibility { get; set; }
    }

    /// <summary>
    /// Information about lists
    /// </summary>
    public class ListResponse
    {
        public List<List> lists { get; set; }
        public int total_items { get; set; }
    }

}
