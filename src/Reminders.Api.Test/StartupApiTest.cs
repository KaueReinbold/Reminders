using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            // TODO: Check if there is a way to work with Environment variables.
            //var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var environment = "Development";

            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .Build();
            
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
