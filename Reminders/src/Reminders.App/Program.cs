using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Reminders.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            try
            {
                host.Run();

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
