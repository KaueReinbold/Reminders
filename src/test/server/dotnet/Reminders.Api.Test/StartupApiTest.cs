using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reminders.Infrastructure.CrossCutting.Configuration;
using System;
using System.IO;
using System.Net.Http;

namespace Reminders.Api.Test
{
    [TestClass]
    public class StartupApiTest
    {
        public IConfiguration Configuration;
        public HttpClient httpClient;
        public readonly string BaseAddress;

        public StartupApiTest()
        {
            Configuration = IConfigurationHelper.GetConfiguration();

            HttpClientHandler clientHandler = new HttpClientHandler();

            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            BaseAddress = Configuration["ApiBaseUrl"];

            httpClient = new HttpClient(clientHandler)
            {
                BaseAddress = new Uri(BaseAddress)
            };
        }
    }
}
