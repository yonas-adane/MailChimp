using MailChimp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailChimp.Domain.Interfaces
{
    interface IReportService<T>
    {

        /// <summary>
        /// Get campaign reports
        /// </summary>
        /// <param name="includeAll"></param>
        /// <returns></returns>
        Report<T> GetReport(bool includeAll = false);

        /// <summary>
        /// Get a specific campaign report
        /// </summary>
        /// <param name="campaignId"></param>
        /// <returns></returns>
        T GetReport(string campaignId);

        /// <summary>
        /// Get information about campaign recipients
        /// </summary>
        /// <param name="campaignId"></param>
        /// <returns></returns>
        Report<T> GetSentTo(string campaignId);

    }
}
