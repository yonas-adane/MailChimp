using System;
using System.Collections.Generic;
using System.Text;

namespace MailChimp.Domain.Models
{
    class SentTo
    {
        /// <summary>
        /// The MD5 hash of the lowercase version of the list member’s email address.
        /// </summary>
        public string email_id { get; set; }

        /// <summary>
        /// Email address for a subscriber.
        /// </summary>
        public string email_address { get; set; }

        /// <summary>
        /// The status of the member (‘sent’, ‘hard’ for hard bounce, or ‘soft’ for soft bounce).
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// The number of times a campaign was opened by this member.
        /// </summary>
        public int open_count { get; set; }

        /// <summary>
        /// The date and time of the last open for this member.
        /// </summary>
        public string last_open { get; set; }

        /// <summary>
        /// For A/B Split Campaigns, the group the member was apart of (‘a’, ‘b’, or ‘winner’).
        /// </summary>
        public string absplit_group { get; set; }

        /// <summary>
        /// For campaigns sent with timewarp, the time zone group the member is apart of.
        /// </summary>
        public int gmt_offset { get; set; }

        /// <summary>
        /// The campaign id.
        /// </summary>
        public string campaign_id { get; set; }

        /// <summary>
        /// The list id.
        /// </summary>
        public string list_id { get; set; }

    }
}
