using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HIT.Web.Startup))]
namespace HIT.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
