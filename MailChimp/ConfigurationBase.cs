using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace MailChimp
{
    /// <summary>
    /// Singlton class + strongly typed configuration
    /// http://csharpindepth.com/Articles/General/Singleton.aspx
    /// </summary>
    public sealed class ConfigurationBase
    {

        public readonly AppSettings appSettings = null;

        public HttpClient httpClient = null;

        private static readonly ConfigurationBase instance = new ConfigurationBase();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static ConfigurationBase()
        {
        }

        private ConfigurationBase()
        {
            //Build strongly typed configuration 
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            appSettings = new AppSettings();

            configuration.GetSection("MailChimp").Bind(appSettings);


            //setup httpClient
            this.httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri(appSettings.APIBaseUrl);

            MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");

            httpClient.DefaultRequestHeaders.Accept.Add(contentType);

            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("" + appSettings.UserName + ":" + appSettings.APIKey));

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        }

        public static ConfigurationBase Instance
        {
            get
            {
                return instance;
            }
        }


    }
}
