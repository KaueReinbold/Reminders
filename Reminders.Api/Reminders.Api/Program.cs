using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Reminders.Api
{
    /// <summary>
    /// Program class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main.
        /// </summary>
        /// <param name="args"> args array. </param>
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        /// <summary>
        /// Build.
        /// </summary>
        /// <param name="args"> args array. </param>
        /// <returns> IWebHost object. </returns>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
