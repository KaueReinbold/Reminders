using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Net.Http;

namespace Reminders.Api.Test
{
    public class StartupApiTest
    {
        public IConfigurationRoot _configuration;
        public ServiceProvider _serviceProvider;
        public IMapper _mapper;
        public HttpClient _httpClient;

        public StartupApiTest()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .Build();

            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }
    }
}
