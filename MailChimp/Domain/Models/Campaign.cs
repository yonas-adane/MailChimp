using System;
using System.Collections.Generic;
using System.Text;

namespace MailChimp.Domain.Models
{
    class Campaign
    {
        /// <summary>
        /// A string that uniquely identifies this campaign.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// The title of the campaign.
        /// </summary>
        public string campaign_title { get; set; }

        /// <summary>
        /// The type of campaign (regular, plain-text, ab_split, rss, automation, variate, or auto).
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// The total number of emails sent for this campaign.
        /// </summary>
        public int emails_sent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int abuse_reports { get; set; }

        /// <summary>
        /// The number of abuse reports generated for this campaign.
        /// </summary>
        public int unsubscribed { get; set; }

        /// <summary>
        /// The total number of unsubscribed members for this campaign.
        /// </summary>
        public string send_time { get; set; }


        ///For this demo, we'll use the above fields
        ///but there are many more campaign report fields providing detailed 
        ///info about a campaign

    }
}
