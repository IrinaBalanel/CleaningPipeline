using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CleaningPipeline.Startup))]
namespace CleaningPipeline
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
