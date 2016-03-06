using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EC_TH2012_J.Startup))]
namespace EC_TH2012_J
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
