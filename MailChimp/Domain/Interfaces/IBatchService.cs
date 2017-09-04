using MailChimp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailChimp.Domain.Interfaces
{
    interface IBatchService<T>
    {

        Batch<T> GetStatus(bool includeAll = false);

        T GetStatus(string batchId);

    }
}
