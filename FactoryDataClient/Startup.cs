using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FactoryDataClient.Startup))]
namespace FactoryDataClient
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
