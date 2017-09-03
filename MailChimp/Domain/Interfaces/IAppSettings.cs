using System;
using System.Collections.Generic;
using System.Text;

namespace MailChimp.Domain.Interfaces
{
    public interface IAppSettings
    {
        string APIBaseUrl { get; }

        string UserName { get; }

        string APIKey { get; }

        string Lists { get; }

        string Reports { get; }

        string Campaigns { get; }

        string Batches { get; } 

    }
}
