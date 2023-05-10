using Microsoft.AspNetCore.Hosting;
using Webima.Areas.Identity;

[assembly: HostingStartup(typeof(IdentityHostingStartup))]

namespace Webima.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((_, _) => { });
        }
    }
}