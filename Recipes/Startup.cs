using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Recipes.Startup))]
namespace Recipes
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
