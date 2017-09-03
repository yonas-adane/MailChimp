using MailChimp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailChimp
{
    public class AppSettings : IAppSettings
    {

        public string APIBaseUrl { get; set; }

        public string UserName { get; set; }

        public string APIKey { get; set; }

        public string Lists { get; set; }

        public string Reports { get; set; }

        public string Campaigns { get; set; }

        public string Batches { get; set; }

    }
}
