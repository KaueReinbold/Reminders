using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Net.Http;

namespace Reminders.Mvc.Test.Api
{
    public class TestConfigurationApi
    {
        public IConfigurationRoot _configuration;
        public ServiceProvider _serviceProvider;
        public IMapper _mapper;
        public HttpClient _httpClient;

        public TestConfigurationApi()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .Build();

            _httpClient = new HttpClient();
        }
    }
}
