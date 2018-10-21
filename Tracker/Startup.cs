using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Tracker.Startup))]
namespace Tracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
